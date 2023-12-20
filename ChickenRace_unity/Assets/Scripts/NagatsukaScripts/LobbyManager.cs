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
    #region ���܂���Name�z��
    private static readonly string[] OMAKASE_NAMES = new string[] { "�}�C�N", "�L���V�[", "������","�W���j�[","�˂���","�����",
        "���񂠂Ȃ�","����"};
    #endregion

    [SerializeField, Tooltip("1���[���̎Q���l��")] int maxPlayer;
    [SerializeField, Tooltip("�Q���{�^�����X�g")] CanvasGroup ButtonRoot;
    [SerializeField, Tooltip("���O���͗p")] InputField inputNickName;    //���O����͂���p.
    [SerializeField, Tooltip("�����̖��O���X�g")] Dropdown roomNameDropdown;
    [SerializeField, Tooltip("�����{�^��")] Button connectRoomButton;
    
    /// <summary>
    /// �R�l�N�g�T�[�o�[����Ă΂�A
    /// ���r�[�ւ̎Q�������s����
    /// </summary>
    public void TryRobyJoin()
    {
        PhotonNetwork.JoinLobby();
    }

    /// <summary>
    /// ���r�[�ڑ����ɌĂ΂��
    /// </summary>
    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        connectRoomButton.interactable = true;
        connectRoomButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "����";
    }

    /// <summary>
    /// �����{�^�����������烋�[���ɓ���.
    /// </summary>
    public void JoinRoom()
    {
        ConectServer.RoomProperties.MaxPlayer = maxPlayer;
        connectRoomButton.interactable = false;                          //���̃��[���̃{�^���������s�ɂ���.
        ConectServer.RoomProperties.RoomName = roomNameDropdown.value.ToString(); //�������郋�[���̖��O��ݒ�.
        PhotonNetwork.NickName = inputNickName.text;
        SceneManager.LoadScene(SceneNames.WaitRoom.ToString());                       //�Q�[���ҋ@�V�[���Ɉړ�.
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
        PhotonNetwork.Disconnect();      //Photon�̃T�[�o�[����ؒf.
        while (PhotonNetwork.IsConnected)//�ڑ����Ă�����ؒf����܂Ń��[�v����.
        {
            yield return null;
        }
        //SceneManager.LoadScene(SceanNames.STARTTITLE.ToString());

    }

    #region �j�b�N�l�[����ݒ�
    public void ChangeNickName()
    {
        int rnd = Random.Range(0, OMAKASE_NAMES.Length);
        inputNickName.text = OMAKASE_NAMES[rnd];
    }
    #endregion

    #region Unity�C�x���g(Start�EUpdate)
    // Start is called before the first frame update
    void Start()
    {
        //�Q�������������r�[�Q���O�͉����Ȃ�����
        connectRoomButton.interactable = false;
        connectRoomButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "�ڑ���";
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    #endregion

    #region �g���Ă��Ȃ����
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        //base.OnRoomListUpdate(roomList);

        //foreach(var room in roomList)
        //{
        //    if (int.Parse(room.Name) < 0 || int.Parse(room.Name) > ButtonRoot.transform.childCount) continue;

        //    //���[���ԍ��ɉ����������X�V
        //    var button = ButtonRoot.transform.GetChild(int.Parse(room.Name));
        //    button.transform.GetChild(0).GetComponent<Text>().text = RoomHead+room.Name+RoomMidle + room.PlayerCount + "/" + room.MaxPlayers 
        //                                                             + "\n"+ (room.IsOpen ? RoomOK : RoomNotOK);

        //    //�����ł��郋�[���̂݃{�^����������悤�ɂ���.
        //    if (room.IsOpen)
        //    {
        //        button.GetComponent<Button>().interactable = true;//�{�^�������\��Ԃ�.
        //    }
        //    else
        //    {
        //        button.GetComponent<Button>().interactable = false;//�{�^�������s��Ԃ�.
        //    }
        //}
    }
    #endregion
}