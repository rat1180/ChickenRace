using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class DataSharingClass : MonoBehaviourPunCallbacks, IPunObservable
{
    public List<int> ID = new List<int>();
    public List<int> score = new List<int>();
    public List<float> rankTime = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.SetDataSheringClass(gameObject.GetComponent<DataSharingClass>());
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
        photonView.RPC(nameof(PushID), RpcTarget.All, iD);
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
        photonView.RPC(nameof(ResetID), RpcTarget.All, index);
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
        photonView.RPC(nameof(ResetIDList), RpcTarget.All);
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
