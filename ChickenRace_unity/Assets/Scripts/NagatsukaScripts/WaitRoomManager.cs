using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using ConstList;

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

    public DataSharingClass dataSharingClass;//�f�[�^���L�N���X.
    public Text sharDataText;

    #region �`���b�g�֘A
    [Header("�`���b�g�֘A")]
    [SerializeField] GameObject ChatGroup;
    [SerializeField] InputField InputChat;
    [SerializeField] Text ChatLog;
    [SerializeField] Text ChatLog2;
    #endregion

    #region �F�萔
    const string RED16 = "#FF0000";
    const string YELLOW16 = "#FFFF00";
    const string BLACK16 = "#000000";
    #endregion

    /// <summary>
    /// �`���b�g�O���[�v�̎q�v�f�̂܂Ƃ�
    /// </summary>
    enum ChatGroups:int { 
        BackGround,
        InputChat,
        ChatLog,
        ChatLog2,
    }


    //�}�X�^�[���ǂ���
    public bool isMaster;
    //���[�����Őڑ��ł��Ă��邩
    private bool isInRoom;
    //�X�^�[�g�������ǂ���
    private bool isStart;

    //���ݕ����ɂ���v���C���[�̐�
    public int nowPlayers;

    private bool createPlayerFlg;//�v���C���[�𐶐�����������.

    //���[���̃J�X�^���v���p�e�B��ݒ肷��ׂ̐錾.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    public GameObject player;

    #region Unity�C�x���g(Start�EUpdate)
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


    #region �`���b�g�E�V�X�e�����b�Z�[�W�@�\�֘A
    public void PushSendChatButton()
    {
        string chat = ChatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text;
        string name = PhotonNetwork.LocalPlayer.NickName;
        ChatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text = "";//���M���������.
        photonView.RPC(nameof(PushChat), RpcTarget.All, name, chat,BLACK16);
    }
    /// <summary>
    /// Player���őł`���b�g�̊֐�.
    /// </summary>
    [PunRPC]
    void PushChat(string name, string chat, string color16)
    {
        Color color;
        ColorUtility.TryParseHtmlString(color16, out color);
        //�O��̃`���b�g���c��.
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().text =
                 ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text;
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().color =
                 ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color;
        //���M���ꂽ�`���b�g�𑗂�.
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text = name + ":" + chat;
        ChatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color = color;
    }
    /// <summary>
    /// �V�X�e�����b�Z�[�W�֘A�̃`���b�g�֐�.
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
    /// �`���b�g�̐F�Ɠ��e�����O�Ɏc��.
    /// </summary>
    private void LeaveChat()
    {

    }

    #endregion


    #region �f�[�^���L�N���X�֘A

    /// <summary>
    /// �f�[�^���L�N���X���������ꂽ�ہA���̊֐����Ă�Œl������.
    /// </summary>
    [PunRPC]
    public void PushDataSharingClass(GameObject gameObject)
    {
        dataSharingClass = gameObject.GetComponent<DataSharingClass>();
    }
    /// <summary>
    /// DataSharingClass�̒l���e�L�X�g�ɕ\������֐�.
    /// </summary>
    private void ShowDataSharing()
    {
        if (dataSharingClass == null) return;//�����o���Ă��Ȃ�������֐����I��.
        sharDataText.text = "";
        //for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)//���݂���l�������[�v����.
        //{
        //    sharDataText.text += "ID:" + dataSharingClass.ID[i].ToString() +
        //                        "�@Score:" + dataSharingClass.score[i].ToString() + "\n";
        //   // Debug.Log("Player�̐l��" + PhotonNetwork.PlayerList.Length);
        //}
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            sharDataText.text += player.NickName +
                                " Score:" + dataSharingClass.score[i].ToString() + "\n";
            // Debug.Log("Player�̐l��" + PhotonNetwork.PlayerList.Length);
            i++;
        }

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
    /// �ޏo�{�^�����������畔���𔲂���.
    /// </summary>
    public void PushExitButton()
    {
        PhotonNetwork.LeaveRoom();// ���[������ޏo����
        SceneManager.LoadScene(SceneNames.Lobby.ToString());  //�Q�[���ҋ@�V�[���Ɉړ�.
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
            //var obj = PhotonNetwork.Instantiate("NagatsukaObjects/DataSharingClass", Vector3.zero, Quaternion.identity);//�f�[�^���L�N���X�𐶐�����.
            //obj.SetActive(true);
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
            if (isMaster)
            {
                SceanMoveButton.interactable = true;
                SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "�Q�[�����n����(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                MessageText.text = "�{�^���������ƃQ�[�����n�܂�܂�";
            }
            else
            {
                SceanMoveButton.interactable = false;
                SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "�J�n���҃b�e��(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                MessageText.text = "�z�X�g�̊J�n���܂��Ă��܂�...";
            }
            if (!createPlayerFlg)//Player�𐶐����Ă��Ȃ���ΐ�������.
            {
                Instantiate(player, Vector3.zero, Quaternion.identity);//Player�𐶐�����.
                createPlayerFlg = true;
            }
            //SceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
            //= "�Q�[�����n����(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";

            //�����o���X�g��\��
        }
    }

    #region�@�Q�[���J�n�{�^���֘A
    /// <summary>
    /// �z�X�g�����������{�^������������Q�[�����n�߂�.
    /// </summary>
    public void PushReadyButton()
    {
        var obj = PhotonNetwork.Instantiate("NagatsukaObjects/DataSharingClass", Vector3.zero, Quaternion.identity);//�f�[�^���L�N���X�𐶐�����.
        PushDataSharingClass(obj);
        photonView.RPC(nameof(PushDataSharingClass), RpcTarget.All,obj);
    }
    #endregion

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
