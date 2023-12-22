using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;
using Photon.Pun;
using Photon.Realtime;


public class UIManager : MonoBehaviour
{
    enum ResultCharacterChild {
        NAME,
        SCORE,
        UPSCORE,
        RANK
    }


    private GameObject imageObjects;
    private GameObject resultPanel;
    private GameObject resultCharacters;//リザルト画面のキャラクター(Name,Score,Rank表示)
    private List<int> id;
    


    public string[] names = new string[4];

    private void Start()
    {
        imageObjects = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        resultPanel = gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        resultCharacters = resultPanel.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PushNameTest();
        }
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    SoldOut(testSoldOutInex);
        //}
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
        for(int i = 0; i < imageObjects.transform.childCount; i++)
        {
            //imageObjects.transform.GetChild(i).GetComponent<Image>().sprite =
            imageObjects.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
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

    /// <summary>
    /// レース終了時リザルト画面を表示するためにGameManagerから呼び出す関数.
    /// </summary>
    public void Result()
    {
        //Playerの数分ループして情報を入れる.
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            resultCharacters.transform.GetChild(i).gameObject.transform.GetChild((int)ResultCharacterChild.NAME).GetComponent<Text>().text = player.NickName;
        }


    }

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
