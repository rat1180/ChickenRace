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
    [SerializeField, Tooltip("����\�����郁�b�Z�[�W")] Text messageText;
    [SerializeField, Tooltip("�J�n�{�^��")] Button sceanMoveButton;

    [SerializeField, Tooltip("ChatGroup�����̂܂ܓ����")] GameObject chatGroup;
    [SerializeField, Tooltip("RoomInformation�����̂܂ܓ����")] GameObject roomInformation;
    [SerializeField, Tooltip("playersImage�����̂܂ܓ����")] GameObject playersImage;

    #region �F�萔
    const string RED16 = "#FF0000";
    const string YELLOW16 = "#FFFF00";
    const string BLACK16 = "#000000";
    #endregion

    /// <summary>
    /// �`���b�g�O���[�v�̎q�v�f�̂܂Ƃ�
    /// </summary>
    private enum ChatGroups { 
        BackGround,
        InputChat,
        ChatLog,
        ChatLog2,
    }

    /// <summary>
    /// ���[���̏���\������I�u�W�F�N�g�̎q�v�f�܂Ƃ�
    /// �f�o�b�O�p
    /// </summary>
    private enum RoomInformations
    {
        RoomNameBack,
        RoomNameText,//���[������\������e�L�X�g.
        PlayerNameBack,
        PlayerNameText,//���[���ɂ���l�̖��O��\��.
    }

   private enum PlayerObjectChild
    {
        Name,
        AnimArm,
        Body,
        AnimLeg,
    }

    private bool isInRoom;       //���[�����Őڑ��ł��Ă��邩
    private bool isStart;        //�X�^�[�g�������ǂ���
    private bool createPlayerFlg;//�v���C���[�𐶐�����������.

    //���[���̃J�X�^���v���p�e�B��ݒ肷��ׂ̐錾.
    ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;//�z�X�g�ƃV�[���𓯊�����.
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


    #region �`���b�g�E�V�X�e�����b�Z�[�W�@�\�֘A
    public void PushSendChatButton()
    {
        string chat = chatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text;
        string name = PhotonNetwork.LocalPlayer.NickName;
        chatGroup.transform.GetChild((int)ChatGroups.InputChat).GetComponent<InputField>().text = "";//���M���������.
        photonView.RPC(nameof(SendChat), RpcTarget.All, name, chat,BLACK16);
    }
    /// <summary>
    /// Player���őł`���b�g�𑗐M����֐�.
    /// </summary>
    [PunRPC]
    void SendChat(string name, string message, string color16)
    {
        string chat = name + ":" + message;
        PushChat(chat, color16);
    }
    /// <summary>
    /// �V�X�e�����b�Z�[�W(���ގ��Ȃ�)�𑗐M����֐�.
    /// </summary>
    [PunRPC]
    void SendChat(string message,string color16)
    {
        PushChat(message, color16);
    }

    /// <summary>
    /// �`���b�g�̐F�Ɠ��e�����O�Ɏc��.
    /// </summary>
    private void PushChat(string chat, string color16)
    {
        Color color;
        ColorUtility.TryParseHtmlString(color16, out color);
        //�O��̃`���b�g���c��.
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().text =
                 chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text;
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog2).GetComponent<Text>().color =
                 chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color;
        //���M���ꂽ�`���b�g�𑗂�.
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().text = chat;
        chatGroup.transform.GetChild((int)ChatGroups.ChatLog).GetComponent<Text>().color = color;
    }

    #endregion

    #region �f�[�^���L�N���X�֘A

    /// <summary>
    /// DataSharingClass�̒l���e�L�X�g�ɕ\������֐�.
    /// �f�o�b�O�p.
    /// </summary>
    private void ShowRoomInformation()
    {
        Debug.Log("���\��");
        //���[���̖��O��\������.
        roomInformation.transform.GetChild((int)RoomInformations.RoomNameText).GetComponent<Text>().text = 
           ConectServer.RoomProperties.RoomName.ToString();
       //roomInformation.transform.GetChild((int)RoomInformations.RoomNameText).GetComponent<Text>().text =
            
        //���[���ɂ���v���C���[�̐���\������.
        roomInformation.transform.GetChild((int)RoomInformations.PlayerNameText).GetComponent<Text>().text = "�Q���v���C���[\n";
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)//�v���C���[�̖��O���擾.
        {
            playersImage.transform.GetChild(i).gameObject.SetActive(true);
            if (player.IsMasterClient)
            {
                roomInformation.transform.GetChild((int)RoomInformations.PlayerNameText).GetComponent<Text>().text +=
                                player.NickName + ":���z�X�g\n";
                playersImage.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text +=
                                player.NickName + ":���z�X�g\n";
            }
            else
            {
                roomInformation.transform.GetChild((int)RoomInformations.PlayerNameText).GetComponent<Text>().text +=
                                player.NickName + "\n";
                playersImage.transform.GetChild(i).transform.GetChild(0).GetComponent<Text>().text +=
                                player.NickName + ":���z�X�g\n";
            } 
        }
    }
    #endregion

    /// <summary>
    /// ���݂̃����o�[�ŃQ�[���V�[���Ɉړ�����
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
    /// ���̃V�[���ړ���ɉ��߂ă��[���ڑ������s����
    /// </summary>
    void TryRoomJoin()
    {
        //�I�t���C���ȊO�̎��ɐڑ�
        if (!ConectServer.RoomProperties.offline)
        {
            var roomOptions = new RoomOptions();
            roomOptions.MaxPlayers = ConectServer.RoomProperties.MaxPlayer;
            
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
        isInRoom = true;//�ڑ�����.
        if (PhotonNetwork.IsMasterClient){}
        PhotonNetwork.AutomaticallySyncScene = true;
        //Debug.Log("OnJoin");

        string message = PhotonNetwork.NickName + "���������܂���";
        photonView.RPC(nameof(SendChat), RpcTarget.All, message,YELLOW16);

        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);
        customProperties.Clear();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        base.OnJoinRoomFailed(returnCode, message);

        messageText.text = message + " �ɂ���Đڑ��o���܂���";
    }

    /// <summary>
    /// ���[�����̏����X�V���A�\������
    /// </summary>
    void RoomStatusUpDate()
    {
            // ���[���̎Q���l����4�l�ɐݒ肷��
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
                = "�Q�[���X�^�[�g(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                messageText.text = "�Z�{�^���ŊJ�n���܂�";
            }
            else
            {
                sceanMoveButton.interactable = false;
                sceanMoveButton.transform.GetChild(0).gameObject.GetComponent<Text>().text
                = "�J�n��҂��Ă��܂�(" + PhotonNetwork.CurrentRoom.PlayerCount + "/" + PhotonNetwork.CurrentRoom.MaxPlayers + ")";
                messageText.text = "�z�X�g�̊J�n���܂��Ă��܂�...";
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

    #region�@�Q�[���J�n�{�^���֘A
    /// <summary>
    /// �z�X�g�����������{�^������������Q�[�����n�߂�.
    /// </summary>
    public void PushReadyButton()
    {
        if (sceanMoveButton.IsInteractable())
        {
            MoveGameScean();
        }
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
        photonView.RPC(nameof(SendChat), RpcTarget.All, message,RED16);
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
