using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
//using ConstList_old;

/*
�}�b�`���O���ɑҋ@���郍�r�[���̊Ǘ��}�l�[�W���[
���݂̃��r�[��Ԃ̕\���ƁA���r�[���̃R���g���[�����s��
�}�b�`���O��̓Q�[���J�n�ɍ��킹�ăV�[���̈ړ����s��
 */
public class WaitRoomManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField, Tooltip("����\�����郁�b�Z�[�W")] Text MessageText;
    [SerializeField, Tooltip("�J�n�{�^��")] Button SceanMoveButton;
    [SerializeField, Tooltip("���[������\������e�L�X�g")] Text roomNameText;
    [SerializeField, Tooltip("���[������\������e�L�X�g")] Text nowPlayerCountText;

    #region �`���b�g�֘A
    [Header("�`���b�g�֘A")]
    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;
    #endregion

    #region �F�萔
    const string RED16 = "#FF0000";
    const string YELLOW16 = "#FFFF00";
    #endregion

    //�}�X�^�[���ǂ���
    public bool isMaster;
    //���[�����Őڑ��ł��Ă��邩
    private bool isInRoom;
    //�X�^�[�g�������ǂ���
    private bool isStart;

    //���ݕ����ɂ���v���C���[�̐�
    public int nowPlayers;

    //���[���̃J�X�^���v���p�e�B��ݒ肷��ׂ̐錾.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    #region Unity�C�x���g(Start�EUpdate)
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


    #region �`���b�g�@�\�֘A
    public void PushSendChatButton()
    {
        string chat = InputChat.text;
        int master = PhotonNetwork.LocalPlayer.ActorNumber;//���M�҂̔ԍ�(�S����ActorNumber���Ă΂Ȃ����߂ɑ��).
        InputChat.text = "";//���M���������.
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
    /// ���݂̃����o�[�ŃQ�[���V�[���Ɉړ�����
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
    /// ���̃V�[���ړ���ɉ��߂ă��[���ڑ������s����
    /// </summary>
    void TryRoomJoin()
    {
        //Debug.Log("RoomName:" + ConectServer.RoomProperties.RoomName);
        //�I�t���C���ȊO�̎��ɐڑ�
        if (ConectServer.RoomProperties.RoomName != "Offline")
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = ConectServer.RoomProperties.MaxPlayer;
            //PhotonNetwork.CurrentRoom.MaxPlayers = (byte)ConectServer.RoomProperties.MaxPlayer;
            //�^�C�g���Ŋm���������Őڑ�
            PhotonNetwork.JoinOrCreateRoom(ConectServer.RoomProperties.RoomName, roomOptions, TypedLobby.Default);
        }
        else
        {
            StartCoroutine(WaitDisconect());
        }
    }

    /// <summary>
    /// �ؒf��҂��Ă���I�t���C�����[�h���J�n����
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
        //�^�C�g���Ŋm���������Őڑ�
        PhotonNetwork.JoinOrCreateRoom(ConectServer.RoomProperties.RoomName, new RoomOptions(), TypedLobby.Default);
    }

    /// <summary>
    /// �ڑ����ɌĂ΂��֐�
    /// �}�X�^�[�̏ꍇ�A�e��ݒ���s��
    /// </summary>
    public override void OnJoinedRoom()
    {
        isMaster = PhotonNetwork.IsMasterClient;
        isInRoom = true;
        if (isMaster)
        {
            // ���[���̎Q���l����2�l�ɐݒ肷��
            //var roomOptions = new RoomOptions();
            //roomOptions.MaxPlayers = ConectServer.RoomProperties.MaxPlayer;
            //PhotonNetwork.CurrentRoom.MaxPlayers = (byte)ConectServer.RoomProperties.MaxPlayer;
            Debug.Log("�ő�l��" + PhotonNetwork.CurrentRoom.MaxPlayers);
        }
        PhotonNetwork.AutomaticallySyncScene = true;
        Debug.Log("OnJoin");


        //�J�X�^���v���p�e�B�̐ݒ�(GAMESTATUS).
        // GAMESTATUS status = GAMESTATUS.NONE;

        //customProperties["GameStatus"] = status;

        string message = PhotonNetwork.NickName + "���������܂���";
        photonView.RPC(nameof(PushChat), RpcTarget.All, message,YELLOW16);

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        MessageText.text = message + " �ɂ���Đڑ��o���܂���";
    }

    /// <summary>
    /// ���[�����̏����X�V���A�\������
    /// </summary>
    void RoomStatusUpDate()
    {
        roomNameText.text = "RoomName:" + ConectServer.RoomProperties.RoomName.ToString();
        nowPlayerCountText.text = "�v���C���[�̐�:"+ PhotonNetwork.PlayerList.Length;
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
                MessageText.text = "�J�n���܂��Ă��܂�...";
            }
            else
            {
                SceanMoveButton.interactable = true;
                MessageText.text = "�X�y�[�X�L�[�������ƃQ�[�����n�܂�܂�";
            }
            SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
            = "�J�n(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

            //�����o���X�g��\��
        }
    }


    // ���[������ޏo�������ɌĂ΂��R�[���o�b�N
    public override void OnLeftRoom()
    {
        Debug.Log("���[������ޏo���܂���");
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        string message = otherPlayer.NickName + "���ޏo���܂���";
        photonView.RPC(nameof(PushChat), RpcTarget.All, message,RED16);
        //Debug.Log(otherPlayer.NickName + "���ޏo���܂����B");
    }

    #region Photon�֘A(�ϐ����M)
    /// <summary>
    /// PUN���g���ĕϐ��𓯊�����.
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
