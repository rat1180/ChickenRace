using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ConstList;


public class LobbyManager : MonoBehaviourPunCallbacks
{
    #region おまかせName配列
    private static readonly string[] OMAKASE_NAMES = new string[] { "マイク", "キャシー","ジョニー","キャン" ,"キャント" ,"チャン" ,
        "ちょこ","ミント","トニー","ジェシカ","ジェシー","キャサリン", "ゴリアテ","カチーナ", "みかん","りんご","きのこ",
        "ねずみ","ごりら","さかな","ちんあなご","いぬ","いっぬ","ねこ","てつこ","まさよし","たかし","きょうこ","なおみ"};
    #endregion


    private string[] roomName = { "〇の部屋", "△の部屋", "□の部屋", "∞の部屋", "はあとのへや", "VIPRoom", "徹子の部屋", "オフライン部屋" };

    const int OFFLINE = 8;

    [SerializeField, Tooltip("1ルームの参加人数")] int maxPlayer;
    [SerializeField, Tooltip("参加ボタンリスト")] CanvasGroup ButtonRoot;
    [SerializeField, Tooltip("名前入力用")] InputField inputNickName;    //名前を入力する用.
    [SerializeField, Tooltip("部屋の名前リスト")] Dropdown roomNameDropdown;
    [SerializeField, Tooltip("入室ボタン")] Button connectRoomButton;
    
    /// <summary>
    /// コネクトサーバーから呼ばれ、
    /// ロビーへの参加を試行する
    /// </summary>
    public void TryRobyJoin()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ロビー接続時に呼ばれる
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        connectRoomButton.interactable = true;
        connectRoomButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "入室";
    }

    /// <summary>
    /// 入室ボタンを押したらルームに入る.
    /// </summary>
    public void JoinRoom()
    {
        ConectServer.RoomProperties.MaxPlayer = maxPlayer;
        connectRoomButton.interactable = false;                          //他のルームのボタンを押下不可にする.
        if (roomNameDropdown.value == OFFLINE)
        {
            ConectServer.RoomProperties.offline = true; //オフラインモード設定.                      
        }
        ConectServer.RoomProperties.RoomName = roomName[roomNameDropdown.value]; //入室するルームの名前を設定.
        if(inputNickName.text == "")//名無しは強制Playerという名前に.
        {
            PhotonNetwork.NickName = "Player" + PhotonNetwork.LocalPlayer.ActorNumber;
        }
        else
        {
            PhotonNetwork.NickName = inputNickName.text;
        }
        
        SceneManager.LoadScene(SceneNames.WaitRoomCP.ToString());//ゲーム待機シーンに移動.
    }

    public void SoloMode()
    {
        ButtonRoot.interactable = false;
        ConectServer.RoomProperties.RoomName = "Offline";
        SceneManager.LoadScene(SceneNames.WaitRoom.ToString());
    }

    public void TitleBack()
    {
        StartCoroutine(WaitDisConect()); 
    }

    IEnumerator WaitDisConect()
    {
        PhotonNetwork.Disconnect();      //Photonのサーバーから切断.
        while (PhotonNetwork.IsConnected)//接続していたら切断するまでループする.
        {
            yield return null;
        }
    }

    #region ニックネームを設定
    public void ChangeNickName()
    {
        int rnd = Random.Range(0, OMAKASE_NAMES.Length);
        inputNickName.text = OMAKASE_NAMES[rnd];
    }
    #endregion

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        //参加処理中かロビー参加前は押せなくする
        connectRoomButton.interactable = false;
        connectRoomButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "接続中";
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    #endregion

    #region 使っていないやつ
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);

        //foreach(var room in roomList)
        //{
        //    if (int.Parse(room.Name) < 0 || int.Parse(room.Name) > ButtonRoot.transform.childCount) continue;

        //    //ルーム番号に応じた情報を更新
        //    var button = ButtonRoot.transform.GetChild(int.Parse(room.Name));
        //    button.transform.GetChild(0).GetComponent<Text>().text = RoomHead+room.Name+RoomMidle + room.PlayerCount + "/" + room.MaxPlayers 
        //                                                             + "\n"+ (room.IsOpen ? RoomOK : RoomNotOK);

        //    //入室できるルームのみボタンを押せるようにする.
        //    if (room.IsOpen)
        //    {
        //        button.GetComponent<Button>().interactable = true;//ボタン押下可能状態に.
        //    }
        //    else
        //    {
        //        button.GetComponent<Button>().interactable = false;//ボタン押下不可状態に.
        //    }
        //}
    }
    #endregion
}