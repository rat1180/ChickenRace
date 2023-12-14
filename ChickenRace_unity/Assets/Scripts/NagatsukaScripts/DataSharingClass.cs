using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DataSharingClass : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<int> ID = new List<int>();
    public List<int> score = new List<int>();
    public List<int> rank = new List<int>();
    public List<float> rankTime = new List<float>();
    public float elapsedTime;//�o�ߎ���.

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.SetDataSheringClass(gameObject.GetComponent<DataSharingClass>());
        //�l�����X�R�A�̓��ꕨ��p�ӂ���.
        for(int i=0;i< ConectServer.RoomProperties.MaxPlayer; i++)
        {
            score.Add(0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))//�f�o�b�O�p
        {
            //1�l�ڂ�0�ԖڂɂȂ�悤��-1(RPC����ActorNumber�������ɂ���ƑS���������Ă�ł��܂����ߑ��).
            int number = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            //photonView.RPC(nameof(PushGoalTime), RpcTarget.All,number,timer);
            photonView.RPC(nameof(PushGoalTime), RpcTarget.All, number, 1);
        }
    }

    /// <summary>
    /// �O������ID���󂯎���ă��X�g�ɓ����֐�
    /// �����ɂ�ID��1���w��(���X�g�̓T�|�[�g����Ă��Ȃ�).
    /// </summary>
    public void PushID(int iD)
    {
        photonView.RPC(nameof(PushIDRPC), RpcTarget.All, iD);
    }

    /// <summary>
    /// ID������RPC.
    /// </summary>
    [PunRPC]
    private void PushIDRPC(int iD)
    {
        ID.Add(iD);
    }

    /// <summary>
    /// �O������index���󂯎���Ďw�肳�ꂽ�ꏊ��0�ɂ���֐�
    /// �����ɂ�index��1���w��.
    /// </summary>
    public void ResetID(int index)
    {
        photonView.RPC(nameof(ResetIDRPC), RpcTarget.All, index);
    }

    /// <summary>
    /// �w�肳�ꂽindex��0�ɂ���RPC.
    /// </summary>
    [PunRPC]
    private void ResetIDRPC(int index)
    {
        ID[index] = 0;
    }

    /// <summary>
    /// �O�����烊�X�g������������֐�.
    /// </summary>
    public void ResetIDList()
    {
        photonView.RPC(nameof(ResetIDListRPC), RpcTarget.All);
    }

    /// <summary>
    /// ���X�g������������RPC.
    /// </summary>
    [PunRPC]
    private void ResetIDListRPC()
    {
        ID = new List<int>();
    }

    /// <summary>
    /// �O�����珇��(Rank)���󂯎���ă��X�g�ɓ����֐�
    /// �����ɂ͎����̏��ʂ��w��.
    /// </summary>
    public void PushRank(int myrank)
    {
        photonView.RPC(nameof(PushRankRPC), RpcTarget.All, myrank);
    }

    /// <summary>
    /// ���ʂ�����RPC.
    /// </summary>
    [PunRPC]
    private void PushRankRPC(int myrank)
    {
        rank.Add(myrank);
    }


    /// <summary>
    /// �o�ߎ��Ԃ�����֐�
    /// </summary>
    public void PushElapsedTime(float time)
    {
        photonView.RPC(nameof(PushElapsedTimeRPC), RpcTarget.All, time);
    }

    /// <summary>
    /// �w�肳�ꂽindex��0�ɂ���RPC.
    /// </summary>
    [PunRPC]
    private void PushElapsedTimeRPC(float time)
    {
        elapsedTime = time;
    }

    /// <summary>
    /// �O������index���󂯎���Ďw�肳�ꂽ�ꏊ��0�ɂ���֐�
    /// �����ɂ�index��1���w��.
    /// </summary>
    public float ReturnTime(int index)
    {
        return rankTime[index];
    }

    /// <summary>
    /// �w�肳�ꂽindex�̒l��Ԃ�RPC.
    /// </summary>
    [PunRPC]
    private void ReturnTimeRPC()
    {
        rankTime = new List<float>();
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            rankTime.Add(0);
        }
    }

    public void PushGoalTime(int index, float time)
    {
        photonView.RPC(nameof(PushGoalTimeRPC), RpcTarget.All,index,time);
    }

    /// <summary>
    /// �S�[�������^�C�~���O�Ŋ֐����Ăяo��
    /// </summary>
    [PunRPC]
    void PushGoalTimeRPC(int index,float time)
    {
        //score[number] += point;
        rankTime[index] = time; 
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
