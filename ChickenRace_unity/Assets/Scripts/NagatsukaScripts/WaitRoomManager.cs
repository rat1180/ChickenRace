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
    [SerializeField, Tooltip("情報を表示するメッセージ")] Text messageText;
    [SerializeField, Tooltip("開始ボタン")] Button sceanMoveButton;

    [SerializeField, Tooltip("ChatGroupをそのまま入れる")] GameObject chatGroup;
    [SerializeField, Tooltip("RoomInformationをそのまま入れる")] GameObject roomInformation;
    [SerializeField, Tooltip("playersImageをそのまま入れる")] GameObject playersImage;

    #region 色定数
    const string RED16 = "#FF0000";
    const string YELLOW16 = "#FFFF00";
    const string BLACK16 = "#000000";
    #endregion

    /// <summary>
    /// チャットグループの子要素のまとめ
    /// </summary>
    private enum ChatGroups { 
        BackGround,
        InputChat,
        ChatLog,
        ChatLog2,
    }

    /// <summary>
    /// ルームの情報を表示するオブジェクトの子要素まとめ
    /// デバッグ用
    /// </summary>
    private enum RoomInformations
    {
        RoomNameBack,
        RoomNameText,//ルーム名を表示するテキスト.
        PlayerNameBack,
        PlayerNameText,//ルームにいる人の名前を表示.
    }

   private enum PlayerObjectChild
    {
        Name,
        AnimArm,
        Body,
        AnimLeg,
    }

    private bool isInRoom;       //ルーム内で接続できているか
    private bool isStart;        //スタートしたかどうか
    private bool createPlayerFlg;//プレイヤーを生成したか判定.

    //ルームのカスタムプロパティを設定する為の宣言.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    #region Unityイベント(Start・Update)
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;//ホストとシーンを同期する.
        TryRoomJoin();
        
        isInRoom = false;
        isStart = false;
        createPlayerFlg = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (isInRoom)
        {
            RoomStatusUpDate();
            if (!PhotonNetwork.OfflineMode)
            {
                ShowRoomInformation();
            }
            
        }
    }
    #endregion


    #region チャット・システムメッセージ機能関連
    public void PushSendChatButton()
    {
        string chat = chatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text;
        string name = PhotonNetwork.LocalPlayer.NickName;
        chatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text = "";//送信したら消す.
        photonView.RPC(nameof(SendChat), RpcTarget.All, name, chat,BLACK16);
    }
    /// <summary>
    /// Player側で打つチャットを送信する関数.
    /// </summary>
    [PunRPC]
    void SendChat(string name, string message, string color16)
    {
        string chat = name + ":" + message;
        PushChat(chat, color16);
    }
    /// <summary>
    /// システムメッセージ(入退室など)を送信する関数.
    /// </summary>
    [PunRPC]
    void SendChat(string message,string color16)
    {
        PushChat(message, color16);
    }

    /// <summary>
    /// チャットの色と内容をログに残す.
    /// </summary>
    private void PushChat(string chat, string color16)
    {
        Color color;
        ColorUtility.TryParseHtmlString(color16, out color);
        //前回のチャットを残す.
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().text =
                 chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text;
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().color =
                 chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color;
        //送信されたチャットを送る.
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text = chat;
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color = color;
    }

    #endregion

    #region データ共有クラス関連

    /// <summary>
    /// DataSharingClassの値をテキストに表示する関数.
    /// デバッグ用.
    /// </summary>
    private void ShowRoomInformation()
    {
        Debug.Log("情報表示");
        //ルームの名前を表示する.
        roomInformation.transform.GetChild((int)RoomInformations.RoomNameText).GetComponent<Text>().text = 
           ConectServer.RoomProperties.RoomName.ToString();
       //roomInformation.transform.GetChild((int)RoomInformations.RoomNameText).GetComponent<Text>().text =
            
        //ルームにいるプレイヤーの数を表示する.
        roomInformation.transform.GetChild((int)RoomInformations.PlayerNameText).GetComponent<Text>().text = "参加プレイヤー\n";
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//プレイヤーの名前を取得.
        {
            playersImage.transform.GetChild(i).gameObject.SetActive(true);
            if (player.IsMasterClient)
            {
                roomInformation.transform.GetChild((int)RoomInformations.PlayerNameText).GetComponent<Text>().text +=
                                player.NickName + ":☆ホスト\n";
                playersImage.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text +=
                                player.NickName + ":☆ホスト\n";
            }
            else
            {
                roomInformation.transform.GetChild((int)RoomInformations.PlayerNameText).GetComponent<Text>().text +=
                                player.NickName + "\n";
                playersImage.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text +=
                                player.NickName + ":☆ホスト\n";
            } 
        }
    }
    #endregion

    /// <summary>
    /// 現在のメンバーでゲームシーンに移動する
    /// </summary>
    public void MoveGameScean()
    {
        if (isStart) return;
        if (PhotonNetwork.IsMasterClient)
        {
            isStart = true;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("GameScean");
        }
    }

    /// <summary>
    /// このシーン移動後に改めてルーム接続を試行する
    /// </summary>
    void TryRoomJoin()
    {
        //オフライン以外の時に接続
        if (!ConectServer.RoomProperties.offline)
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = ConectServer.RoomProperties.MaxPlayer;
            
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
        isInRoom = true;//接続成功.
        if (PhotonNetwork.IsMasterClient){}
        PhotonNetwork.AutomaticallySyncScene = true;
        //Debug.Log("OnJoin");

        string message = PhotonNetwork.NickName + "が入室しました";
        photonView.RPC(nameof(SendChat), RpcTarget.All, message,YELLOW16);

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        messageText.text = message + " によって接続出来ません";
    }

    /// <summary>
    /// ルーム内の情報を更新し、表示する
    /// </summary>
    void RoomStatusUpDate()
    {
            // ルームの参加人数を4人に設定する
            if (PhotonNetwork.CurrentRoom.PlayerCount == ConectServer.RoomProperties.MaxPlayer ||
            PhotonNetwork.OfflineMode)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
            }
            else
            {
                PhotonNetwork.CurrentRoom.IsOpen = true;
            }
    
        
        if (!isInRoom)
        {
            sceanMoveButton.interactable = false;
        }
        else
        {
            if (PhotonNetwork.IsMasterClient)
            {
                sceanMoveButton.interactable = true;
                sceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "ゲームスタート(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                messageText.text = "〇ボタンで開始します";
            }
            else
            {
                sceanMoveButton.interactable = false;
                sceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "開始を待っています(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                messageText.text = "ホストの開始をまっています...";
            }
        }
    }

    public void Connect()
    {
        if (!ConectServer.RoomProperties.offline)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            PhotonNetwork.OfflineMode = true;
        }
    }

    public void Disconnect()
    {
        if (!ConectServer.RoomProperties.offline)
        {
            PhotonNetwork.Disconnect();
        }
        else
        {
            PhotonNetwork.OfflineMode = false;
        }
    }

    #region　ゲーム開始ボタン関連
    /// <summary>
    /// ホストが準備完了ボタンを押したらゲームを始める.
    /// </summary>
    public void PushReadyButton()
    {
        if (sceanMoveButton.IsInteractable())
        {
            MoveGameScean();
        }
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
        photonView.RPC(nameof(SendChat), RpcTarget.All, message,RED16);
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
