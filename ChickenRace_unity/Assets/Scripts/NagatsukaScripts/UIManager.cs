using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;
using Photon.Pun;
using Photon.Realtime;


public class UIManager : MonoBehaviourPunCallbacks
{
    #region 列挙型
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
        LoaingImage
    }
    #endregion

    #region 定数値
    const int IMAGE_OBSTACLE_NUM = 4;//選択する障害物の画像の数.
    #endregion

    private Vector2[] imagePosition =
    {
        new Vector2(-5,3),
        new Vector2(5, 3),
        new Vector2(-5,-5),
        new Vector2(5,-5)
    };

    private GameObject imageObjects;
    private GameObject resultPanel;
    private GameObject resultCharacters;//リザルト画面のキャラクター(Name,Score,Rank表示)
    private Text raceCountText;//第何レースかを表示するテキスト.
    private List<int> id;

    public GameObject imageObstacle;

    //Loadingアニメーション関連.
    private GameObject loaingImage;


    [Header("デバッグ用(実際のゲームでは使用しない)")]
    public string[] names = new string[3];
    public List<int> beScore;
    public List<int> addscoreTest;
    public List<int> testID;

    private void Awake()
    {
        imageObjects = transform.Find("ImageObjects").gameObject;
        resultPanel = transform.GetChild(0).transform.Find("ResultPanel").gameObject;
        resultCharacters = resultPanel.transform.GetChild(0).gameObject;
        raceCountText= resultPanel.transform.GetChild(1).GetComponent<Text>();
        loaingImage = transform.GetChild(0).transform.GetChild((int)CanvasChild.LoaingImage).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //PushID(testID);
            //StartCoroutine(Result(beScore, addscoreTest));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            //FinishSelect();
            //StartCoroutine(Result(beScore, addscoreTest));
        }
    }

    /// <summary>
    /// 全員が障害物の選択を終了したらUIを非表示にする.
    /// </summary>
    public void FinishSelect()
    {
        //imageObjects.SetActive(false);
        for (int i = 0; i < IMAGE_OBSTACLE_NUM; i++)//子要素全削除(childCountにすると数が減ってindexoutエラーを起こすため直接指定).
        {
            Destroy(imageObjects.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// GameManagerからIDを入れる関数
    /// </summary>
    public void PushID(List<int> iD)
    {
        id = new List<int>();
        id = iD;
        //imageObjects.SetActive(true);

        ChangeObstacleImage();
    }

    /// <summary>
    /// 4つの画像を変える
    /// ここでIDも付与する
    /// </summary>
    public void ChangeObstacleImage()
    {
        //for (int i = 0; i < imageObjects.transform.childCount; i++)
        for (int i = 0; i < IMAGE_OBSTACLE_NUM; i++)
        {
            if (imageObjects.transform.childCount != IMAGE_OBSTACLE_NUM)//4つの画像オブジェクトが生成されているかチェック.
            {
                GameObject gameObject = Instantiate(imageObstacle, new Vector3(), Quaternion.identity, imageObjects.transform);
                gameObject.transform.position = imagePosition[i];
                //gameObject.GetComponent<RectTransform>().localPosition = imagePosition[i];

                gameObject.GetComponent<SpriteRenderer>().sprite =
                ResourceManager.instance.GetObstacleImage(id[i]);//画像を変更.
                gameObject.AddComponent<ObstacleImage>().id = i;
            }
            else//4つ生成していたらID情報のみを変更する.
            {
                imageObjects.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite = 
                ResourceManager.instance.GetObstacleImage(id[i]);//画像を変更.

                //コンポーネントが存在すればそのままIDを代入する
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
        imageObjects.transform.GetChild(index).GetComponent<ObstacleImage>().id = 0;
    }

    #region リザルト変更関連

    public IEnumerator Result(List<int> beforeScore, List<int> addScore)
    {
        Debug.Log("GameManagerの916行目付近のバグ対策用、修正したら関数ごと消す");
        return null;
    }

    /// <summary>
    /// レース終了時リザルト画面を表示するためにGameManagerから呼び出すコルーチン
    /// 引数に変更前のスコア・加算するするスコア、レース数を指定.
    /// </summary>
    public IEnumerator Result(List<int> beforeScore, List<int> addScore,int raceCount)
    {
        ActiveResultPanel(true);
        ActiveCharacters(beforeScore.Count);
        raceCountText.text = "第" + raceCount + "レース終了結果";
        //Playerの数分ループして情報を入れる.
        int i = 0;
        for (i = 0; i < beforeScore.Count; i++)
        {
            //foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
            {
                //resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
                resultCharacters.transform.GetChild(i).transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text
                    = names[i];
                resultCharacters.transform.GetChild(i).transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                    = "SCORE:" + beforeScore[i].ToString();
                resultCharacters.transform.GetChild(i).transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + addScore[i].ToString();
            }
        }

        //スコアのテキストを変更する(旧ChangeScoreText).
        List<int> score = beforeScore;
        List<int> adscore = addScore;
        int cnt = 0;
        while (true)//全プレイヤーの加算が終わるまでループする(抜ける際はbreak).
        {
            for (i = 0; i < score.Count; i++)
            {
                if (adscore[i] >= 0)//加算する分のスコアが0でなければテキスト変更する.
                {
                    adscore[i]--;//加算した分-1する.
                    score[i]++;  //引いた分+1する.

                    //スコアのテキストを変更する.
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.SCORE).GetComponent<Text>().text
                            = "SCORE:" + score[i].ToString();
                    resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.UPSCORE).GetComponent<Text>().text
                    = "+" + adscore[i].ToString();
                    if (adscore[i] <= 0)//加算するスコアが0になったらカウントを増やす.
                    {
                        cnt++;
                    }
                }
            }

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
        ActiveResultPanel(false);
        Debug.Log("コルーチンしゅうりょう☆");
        yield return new WaitForSeconds(0.1f);

    }



    /// <summary>
    /// Playerの数分キャラクターを表示させる関数
    /// 引数にPlayerの数を指定.
    /// </summary>
    private void ActiveCharacters(int cnt)
    {
        for (int i = 0; i < 3; i++)
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
    /// スコアに応じて順位を変更する関数
    /// 引数に変更後のスコアを指定.
    /// </summary>
    void ChangeRank(List<int> score)
    {
        for (int i = 0; i < score.Count; i++)
        {
            int rank = 1;
            for (int j = 0; j < score.Count; j++)
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

    /// <summary>
    /// 通信中アニメ―ションの表示・非表示を行う関数
    /// 引数にtrue・falseを指定し、切り換える
    /// </summary>
    /// <param name="flg"></param>
    public void ActiveLoaingImage(bool flg)
    {
        loaingImage.SetActive(flg);
    }



    void PushNameTest()
    {
        //Playerの数分ループして情報を入れる.
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = player.NickName;
        }
    }
}
