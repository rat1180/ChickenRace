using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DataSharingClass : MonoBehaviourPunCallbacks, IPunObservable
{
    WaitRoomManager waitRoomManager;

    public List<int> ID = new List<int>();
    public List<int> score = new List<int>();

    public List<float> rankTime = new List<float>();

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        //�l�����X�R�A�̓��ꕨ��p�ӂ���.
        for(int i=0;i< ConectServer.RoomProperties.MaxPlayer; i++)
        {
            score.Add(0);
            ID.Add(i);//�f�o�b�O�p.
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //photonView.RPC(nameof(PushSharData), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.K))//�f�o�b�O�p
        {
            //1�l�ڂ�0�ԖڂɂȂ�悤��-1(RPC����ActorNumber�������ɂ���ƑS���������Ă�ł��܂����ߑ��).
            int number = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            photonView.RPC(nameof(PushGoalTime), RpcTarget.All,number,timer);
        }
    }

    [PunRPC]
    void PushSharData()
    {
        waitRoomManager = GameObject.Find("WaitRoomManager").GetComponent<WaitRoomManager>();
        waitRoomManager.PushDataSharingClass(gameObject);
    }

    /// <summary>
    /// �X�R�A���Z�^�C�~���O�ł��̊֐����Ăяo��
    /// ������ActorNumber�A�v���X����X�R�A�̐��l���w��
    /// </summary>
    [PunRPC]
    void PushGoalTime(int number,int point)
    {
        score[number] += point;
    }

    /// <summary>
    /// PUN���g���ĕϐ��𓯊�����.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)//���M.
        //{
        //    stream.SendNext(ID[PhotonNetwork.LocalPlayer.ActorNumber]);
        //    stream.SendNext(score[PhotonNetwork.LocalPlayer.ActorNumber]);
        //}
        //else//��M.
        //{
        //    ID = (List<int>)stream.ReceiveNext();
        //    score = (List<int>)stream.ReceiveNext();
        //}
    }
}
