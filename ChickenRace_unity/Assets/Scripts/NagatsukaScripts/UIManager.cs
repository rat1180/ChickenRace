using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;
using Photon.Pun;
using Photon.Realtime;


public class UIManager : MonoBehaviourPunCallbacks
{
    enum ResultCharacterChild
    {
        NAME,
        SCORE,
        UPSCORE,
        RANK,
    }

    enum CanvasChild
    {
        ImageObjects,
        RaceCount,
        ResultPanel,
    }

    private GameObject imageObjects;
    private GameObject resultPanel;
    private GameObject resultCharacters;//リザルト画面のキャラクター(Name,Score,Rank表示)
    private List<int> id;


    [Header("デバッグ用(実際のゲームでは使用しない)")]
    public string[] names = new string[3];
    public List<int> beScore;
    public List<int> addscoreTest;
    public List<OBSTACLE_OBJECT> testID;

    private void Start()
    {
        imageObjects = transform.GetChild(0).transform.GetChild((int)CanvasChild.ImageObjects).gameObject;
        resultPanel =  transform.GetChild(0).transform.GetChild((int)CanvasChild.ResultPanel).gameObject;
        resultCharacters = resultPanel.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //List<int> test = testID;
            //PushID(testID);
            StartCoroutine(Result(beScore, addscoreTest));
        }
    }

    /// <summary>
    /// 全員が障害物の選択を終了したらUIを非表示にする.
    /// </summary>
    public void FinishSelect()
    {
        imageObjects.SetActive(false);
    }

    /// <summary>
    /// GameManagerからIDを入れる関数
    /// </summary>
    public void PushID(List<int> iD)
    {
        id = new List<int>();
        id = iD;
        imageObjects.SetActive(true);
        ChangeObstacleImage();
    }

    /// <summary>
    /// 4つの画像を変える
    /// ここでIDも付与する
    /// </summary>
    public void ChangeObstacleImage()
    {
        for (int i = 0; i < imageObjects.transform.childCount; i++)
        {
            imageObjects.transform.GetChild(i).GetComponent<Image>().sprite =
            //imageObjects.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
            ResourceManager.instance.GetObstacleImage(id[i]);//画像を変更.

            //コンポーネントが存在しなければ追加してIDを代入し、逆に存在すればそのままIDを代入する
            if (imageObjects.transform.GetChild(i).gameObject.GetComponent<ObstacleImage>() == null)
            {
                imageObjects.transform.GetChild(i).gameObject.AddComponent<ObstacleImage>().id = i;
            }
            else
            {
                imageObjects.transform.GetChild(i).gameObject.GetComponent<ObstacleImage>().id = i;
            }
        }
    }

    /// <summary>
    /// indexを指定して完売状態(0)にする
    /// </summary>
    public void SoldOut(int index)
    {
        imageObjects.transform.GetChild(index).GetComponent<Image>().sprite =
            ResourceManager.instance.GetObstacleImage(OBSTACLE_IMAGE_NAMES.Kanbaipop);//完売画像に変更.
        imageObjects.transform.GetChild(index).gameObject.GetComponent<ObstacleImage>().id = 0;
    }

    #region リザルト変更関連

    /// <summary>
    /// レース終了時リザルト画面を表示するためにGameManagerから呼び出すコルーチン
    /// 引数に変更前のスコア・加算するするスコアを指定.
    /// </summary>
    IEnumerator Result(List<int> beforeScore, List<int> addScore)
    {
        ActiveCharacters(names.Length);
        //Playerの数分ループして情報を入れる.
        int i = 0;
        for (i = 0; i < beforeScore.Count; i++)
        {
            //foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
            {
                //resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
                    = names[i];
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                    = "SCORE:" + beforeScore[i].ToString();
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + addScore[i].ToString();
            }
        }
        //StartCoroutine(ChangeScoreText(beforeScore, addScore));

        //スコアのテキストを変更する(旧ChangeScoreText).
        List<int> score = beforeScore;
        List<int> adscore = addScore;
        int cnt = 0;
        while (true)
        {
            for (i = 0; i < score.Count; i++)
            {
                if (adscore[i] != 0)//加算する分のスコアが0でなければテキスト変更する.
                {
                    adscore[i]--;//加算した分-1する.
                    score[i]++;  //引いた分+1する.

                    //スコアのテキストを変更する.
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                            = "SCORE:" + score[i].ToString();
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + adscore[i].ToString();
                    if (adscore[i] == 0)//加算するスコアが0になったらカウントを増やす.
                    {
                        cnt++;
                    }
                }
            }
            //break;

            yield return new WaitForSeconds(0.1f);
            if (cnt == score.Count)//最大人数分全てのスコア加算が終了したらループを抜けて順位を表示する.
            {
                ChangeRank(score);
                break;
            }
        }


        yield return new WaitForSeconds(3f);
        ActiveResultPanel(false);
        //終了処理書くならココ

        Debug.Log("コルーチンしゅうりょう☆");
        yield return new WaitForSeconds(0.1f);

    }



  //  public void Result(List<int> beforeScore, List<int> addScore)
//    {
        //ActiveCharacters(names.Length);
        ////Playerの数分ループして情報を入れる.
        //int i = 0;
        //for (i = 0; i < beforeScore.Count; i++)
        //{
        //    //foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        //    {
        //        //resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
        //        resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
        //            = names[i];
        //        resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
        //            = "SCORE:" + beforeScore[i].ToString();
        //        resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
        //            = "+" + addScore[i].ToString();
        //    }       
        //}
        //StartCoroutine(ChangeScoreText(beforeScore, addScore));
    //}

    private void ActiveCharacters(int cnt)
    {
        //for(int i=0;i< ConectServer.RoomProperties.MaxPlayer; i++)
        for (int i = 0; i < 3 ; i++)
        {
            
            if (i < cnt)
            {
                resultCharacters.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                resultCharacters.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// リザルトパネルのスコアを変化させる関数
    /// 引数に変更前のスコア・加算するするスコアを指定
    /// </summary>
    IEnumerator ChangeScoreText(List<int> beforeScore, List<int> addScore)
    {
        List<int> score = beforeScore;                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                               
        List<int> adscore =  addScore;
        int cnt = 0;
        while (true)
        {
            for (int i = 0; i < score.Count; i++)
            {
                if (adscore[i] != 0)//加算する分のスコアが0でなければテキスト変更する.
                {
                    adscore[i]--;//加算した分-1する.
                    score[i]++;  //引いた分+1する.

                    //スコアのテキストを変更する.
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                            = "SCORE:" + score[i].ToString();
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + adscore[i].ToString();
                    if (adscore[i] == 0)//加算するスコアが0になったらカウントを増やす.
                    {
                        cnt++;
                    }
                }
            }
            //break;

                yield return new WaitForSeconds(0.1f);
            if (cnt == score.Count)//最大人数分全てのスコア加算が終了したらループを抜けて順位を表示する.
            {
                ChangeRank(score);
                break;
            }
        }
        

        yield return new WaitForSeconds(3f);
        //Debug.Log("コルーチン終了");
        ActiveResultPanel(false);
        //終了処理書くならココ

    }

    void ChangeRank(List<int> score)
    {

        for (int i = 0; i < 3; i++)
        {
            int rank = 1;
            for(int j = 0; j < 3; j++)
            {
                if (score[i] < score[j])//自分も比べる事になるが、問題ない,
                {
                    //Debug.Log(i + "番目" + "Score1:" + score[i] + "と" + "Score2" + score[j + 1]);
                    rank++;
                }
            }
            resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.RANK).GetComponent<Text>().text
                    = rank.ToString() + "位";
        }
        
    }

    /// <summary>
    /// リザルトパネルの表示非表示を行う関数
    /// 引数に切り換えたい方(trune・false)を指定
    /// </summary>
    /// <param name="flg"></param>
    private void ActiveResultPanel(bool flg)
    {
        resultPanel.SetActive(flg);
    }

    #endregion

    void PushNameTest()
        {
            //for(int i = 0; i < 3; i++)
            //{
            //    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = names[i];
            //}

            //Playerの数分ループして情報を入れる.
            int i = 0;
            foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
            {
                resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
            }
        }
}
