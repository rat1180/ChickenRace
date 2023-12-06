using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
//using ConstList_old;

/*
マッチング中に待機するロビー内の管理マネージャー
現在のロビー状態の表示と、ロビー情報のコントロールを行う
マッチング後はゲーム開始に合わせてシーンの移動を行う
 */
public class WaitRoomManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField, Tooltip("情報を表示するメッセージ")] Text MessageText;
    [SerializeField, Tooltip("開始ボタン")] Button SceanMoveButton;
    [SerializeField, Tooltip("ルーム名を表示するテキスト")] Text roomNameText;
    [SerializeField, Tooltip("ルーム名を表示するテキスト")] Text nowPlayerCountText;

    #region チャット関連
    [Header("チャット関連")]
    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;
    #endregion

    #region 色定数
    const string RED16 = "#FF0000";
    const string YELLOW16 = "#FFFF00";
    #endregion

    //マスターかどうか
    public bool isMaster;
    //ルーム内で接続できているか
    private bool isInRoom;
    //スタートしたかどうか
    private bool isStart;

    //現在部屋にいるプレイヤーの数
    public int nowPlayers;

    //ルームのカスタムプロパティを設定する為の宣言.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        isMaster = false;
        isInRoom = false;
        isStart = false;
        TryRoomJoin();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRoom)
        {
            RoomStatusUpDate();
            if (Input.GetKeyDown(KeyCode.Space) && SceanMoveButton.IsInteractable())
            {
                MoveGameScean();
            }
        }
    }
    #endregion


    #region チャット機能関連
    public void PushSendChatButton()
    {
        string chat = InputChat.text;
        int master = PhotonNetwork.LocalPlayer.ActorNumber;//送信者の番号(全員がActorNumberを呼ばないために代入).
        InputChat.text = "";//送信したら消す.
        photonView.RPC(nameof(PushChat), RpcTarget.All, master, chat);
    }

    //[PunRPC]
    //void PushChat(string chat)
    //{
    //    ChatLog2.text = ChatLog.text;
    //    ChatLog2.color = ChatLog.color;
    //    ChatLog.text = chat;
    //    ChatLog.color = Color.black;
    //}
    [PunRPC]
    void PushChat(string chat,string color16)
    {
        Color color;
        ColorUtility.TryParseHtmlString(color16, out color);
        ChatLog2.text = ChatLog.text;
        ChatLog2.color = ChatLog.color;
        ChatLog.text = chat;
        ChatLog.color = color;
    }

    #endregion

    /// <summary>
    /// 現在のメンバーでゲームシーンに移動する
    /// </summary>
    public void MoveGameScean()
    {
        if (isStart) return;
        if (isMaster)
        {
            isStart = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
           // PhotonNetwork.LoadLevel(/*"ProttypeSeacn");*/SceanNames.GAME.ToString());
        }
    }

    /// <summary>
    /// このシーン移動後に改めてルーム接続を試行する
    /// </summary>
    void TryRoomJoin()
    {
        //Debug.Log("RoomName:" + ConectServer.RoomProperties.RoomName);
        //オフライン以外の時に接続
        if (ConectServer.RoomProperties.RoomName != "Offline")
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = ConectServer.RoomProperties.MaxPlayer;
            //PhotonNetwork.CurrentRoom.MaxPlayers = (byte)ConectServer.RoomProperties.MaxPlayer;
            //タイトルで確立した情報で接続
            PhotonNetwork.JoinOrCreateRoom(ConectServer.RoomProperties.RoomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            StartCoroutine(WaitDisconect());
        }
    }

    /// <summary>
    /// 切断を待ってからオフラインモードを開始する
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitDisconect()
    {
        PhotonNetwork.Disconnect();
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }
        PhotonNetwork.OfflineMode = true;
        //タイトルで確立した情報で接続
        PhotonNetwork.JoinOrCreateRoom(ConectServer.RoomProperties.RoomName, new RoomOptions(), TypedLobby.Default);
    }

    /// <summary>
    /// 接続時に呼ばれる関数
    /// マスターの場合、各種設定を行う
    /// </summary>
    public override void OnJoinedRoom()
    {
        isMaster = PhotonNetwork.IsMasterClient;
        isInRoom = true;
        if (isMaster)
        {
            // ルームの参加人数を2人に設定する
            //var roomOptions = new RoomOptions();
            //roomOptions.MaxPlayers = ConectServer.RoomProperties.MaxPlayer;
            //PhotonNetwork.CurrentRoom.MaxPlayers = (byte)ConectServer.RoomProperties.MaxPlayer;
            Debug.Log("最大人数" + PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("OnJoin");


        //カスタムプロパティの設定(GAMESTATUS).
        // GAMESTATUS status = GAMESTATUS.NONE;

        //customProperties["GameStatus"] = status;

        string message = PhotonNetwork.NickName + "が入室しました";
        photonView.RPC(nameof(PushChat), RpcTarget.All, message,YELLOW16);

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        MessageText.text = message + " によって接続出来ません";
    }

    /// <summary>
    /// ルーム内の情報を更新し、表示する
    /// </summary>
    void RoomStatusUpDate()
    {
        roomNameText.text = "RoomName:" + ConectServer.RoomProperties.RoomName.ToString();
        nowPlayerCountText.text = "プレイヤーの数:"+ PhotonNetwork.PlayerList.Length;
        //if (PhotonNetwork.CurrentRoom.PlayerCount >= PhotonNetwork.CurrentRoom.MaxPlayers)
        //{
        //    //PhotonNetwork.CurrentRoom.IsOpen = false;
        //}

        if (!isInRoom)
        {
            SceanMoveButton.interactable = false;
        }
        else
        {
            if (!isMaster)
            {
                SceanMoveButton.interactable = false;
                MessageText.text = "開始をまっています...";
            }
            else
            {
                SceanMoveButton.interactable = true;
                MessageText.text = "スペースキーを押すとゲームが始まります";
            }
            SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
            = "開始(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

            //メンバリストを表示
        }
    }


    // ルームから退出した時に呼ばれるコールバック
    public override void OnLeftRoom()
    {
        Debug.Log("ルームから退出しました");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        string message = otherPlayer.NickName + "が退出しました";
        photonView.RPC(nameof(PushChat), RpcTarget.All, message,RED16);
        //Debug.Log(otherPlayer.NickName + "が退出しました。");
    }

    #region Photon関連(変数送信)
    /// <summary>
    /// PUNを使って変数を同期する.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
        }
        else
        {
        }
    }
#endregion
}
