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
            waitRoomManager = GameObject.Find("WaitRoomManager").GetComponent<WaitRoomManager>();
            waitRoomManager.PushDataSharingClass(gameObject);
        }
        if (Input.GetKeyDown(KeyCode.K))//�f�o�b�O�p
        {
            score[PhotonNetwork.LocalPlayer.ActorNumber -1] += 1;//1�l�ڂ�0�ԖڂɂȂ�悤��-1.
        }
    }


    /// <summary>
    /// PUN���g���ĕϐ��𓯊�����.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//���M.
        {
            stream.SendNext(ID[PhotonNetwork.LocalPlayer.ActorNumber]);
            stream.SendNext(score[PhotonNetwork.LocalPlayer.ActorNumber]);
        }
        else//��M.
        {
            ID = (List<int>)stream.ReceiveNext();
            score = (List<int>)stream.ReceiveNext();
        }
    }
}
