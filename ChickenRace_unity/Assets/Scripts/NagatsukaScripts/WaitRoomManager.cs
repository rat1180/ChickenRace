using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ConstList;

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

    public DataSharingClass dataSharingClass;//データ共有クラス.
    public Text sharDataText;

    #region チャット関連
    [Header("チャット関連")]
    [SerializeField] GameObject ChatGroup;
    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;
    #endregion

    #region 色定数
    const string RED16 = "#FF0000";
    const string YELLOW16 = "#FFFF00";
    const string BLACK16 = "#000000";
    #endregion

    /// <summary>
    /// チャットグループの子要素のまとめ
    /// </summary>
    enum ChatGroups:int { 
        BackGround,
        InputChat,
        ChatLog,
        ChatLog2,
    }


    //マスターかどうか
    public bool isMaster;
    //ルーム内で接続できているか
    private bool isInRoom;
    //スタートしたかどうか
    private bool isStart;

    //現在部屋にいるプレイヤーの数
    public int nowPlayers;

    private bool createPlayerFlg;//プレイヤーを生成したか判定.

    //ルームのカスタムプロパティを設定する為の宣言.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public GameObject player;

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        isMaster = false;
        isInRoom = false;
        isStart = false;
        createPlayerFlg = false;
        TryRoomJoin();
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRoom)
        {
            RoomStatusUpDate();
            ShowDataSharing();
            if (Input.GetKeyDown(KeyCode.Space) && SceanMoveButton.IsInteractable())
            {
                MoveGameScean();
            }
        }
    }
    #endregion


    #region チャット・システムメッセージ機能関連
    public void PushSendChatButton()
    {
        string chat = ChatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text;
        string name = PhotonNetwork.LocalPlayer.NickName;
        ChatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text = "";//送信したら消す.
        photonView.RPC(nameof(PushChat), RpcTarget.All, name, chat,BLACK16);
    }
    /// <summary>
    /// Player側で打つチャットの関数.
    /// </summary>
    [PunRPC]
    void PushChat(string name, string chat, string color16)
    {
        Color color;
        ColorUtility.TryParseHtmlString(color16, out color);
        //前回のチャットを残す.
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().text =
                 ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text;
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().color =
                 ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color;
        //送信されたチャットを送る.
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text = name + ":" + chat;
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color = color;
    }
    /// <summary>
    /// システムメッセージ関連のチャット関数.
    /// </summary>
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

    /// <summary>
    /// チャットの色と内容をログに残す.
    /// </summary>
    private void LeaveChat()
    {

    }

    #endregion


    #region データ共有クラス関連

    /// <summary>
    /// データ共有クラスが生成された際、この関数を呼んで値を入れる.
    /// </summary>
    [PunRPC]
    public void PushDataSharingClass(GameObject gameObject)
    {
        dataSharingClass = gameObject.GetComponent<DataSharingClass>();
    }
    /// <summary>
    /// DataSharingClassの値をテキストに表示する関数.
    /// </summary>
    private void ShowDataSharing()
    {
        if (dataSharingClass == null) return;//生成出来ていなかったら関数を終了.
        sharDataText.text = "";
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)//現在いる人数分ループする.
        //{
        //    sharDataText.text += "ID:" + dataSharingClass.ID[i].ToString() +
        //                        "　Score:" + dataSharingClass.score[i].ToString() + "\n";
        //   // Debug.Log("Playerの人数" + PhotonNetwork.PlayerList.Length);
        //}
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            sharDataText.text += player.NickName +
                                " Score:" + dataSharingClass.score[i].ToString() + "\n";
            // Debug.Log("Playerの人数" + PhotonNetwork.PlayerList.Length);
            i++;
        }

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
    /// 退出ボタンを押したら部屋を抜ける.
    /// </summary>
    public void PushExitButton()
    {
        PhotonNetwork.LeaveRoom();// ルームから退出する
        SceneManager.LoadScene(SceneNames.Lobby.ToString());  //ゲーム待機シーンに移動.
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
            //var obj = PhotonNetwork.Instantiate("NagatsukaObjects/DataSharingClass", Vector3.zero, Quaternion.identity);//データ共有クラスを生成する.
            //obj.SetActive(true);
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
            if (isMaster)
            {
                SceanMoveButton.interactable = true;
                SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "ゲームヲ始メル(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                MessageText.text = "ボタンを押すとゲームが始まります";
            }
            else
            {
                SceanMoveButton.interactable = false;
                SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "開始ヲ待ッテル(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                MessageText.text = "ホストの開始をまっています...";
            }
            if (!createPlayerFlg)//Playerを生成していなければ生成する.
            {
                Instantiate(player, Vector3.zero, Quaternion.identity);//Playerを生成する.
                createPlayerFlg = true;
            }
            //SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
            //= "ゲームを始メル(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

            //メンバリストを表示
        }
    }

    #region　ゲーム開始ボタン関連
    /// <summary>
    /// ホストが準備完了ボタンを押したらゲームを始める.
    /// </summary>
    public void PushReadyButton()
    {
        var obj = PhotonNetwork.Instantiate("NagatsukaObjects/DataSharingClass", Vector3.zero, Quaternion.identity);//データ共有クラスを生成する.
        PushDataSharingClass(obj);
        photonView.RPC(nameof(PushDataSharingClass), RpcTarget.All,obj);
    }
    #endregion

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
