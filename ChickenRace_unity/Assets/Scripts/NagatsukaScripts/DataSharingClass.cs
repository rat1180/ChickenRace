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
        //人数分スコアの入れ物を用意する.
        for(int i=0;i< ConectServer.RoomProperties.MaxPlayer; i++)
        {
            score.Add(0);
            ID.Add(i);//デバッグ用.
        }
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            //photonView.RPC(nameof(PushSharData), RpcTarget.All);
        }
        if (Input.GetKeyDown(KeyCode.K))//デバッグ用
        {
            //1人目が0番目になるように-1(RPC内でActorNumberを引数にすると全員自分を呼んでしまうため代入).
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
    /// スコア加算タイミングでこの関数を呼び出す
    /// 引数にActorNumber、プラスするスコアの数値を指定
    /// </summary>
    [PunRPC]
    void PushGoalTime(int number,int point)
    {
        score[number] += point;
    }

    /// <summary>
    /// PUNを使って変数を同期する.
    /// </summary>
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)//送信.
        //{
        //    stream.SendNext(ID[PhotonNetwork.LocalPlayer.ActorNumber]);
        //    stream.SendNext(score[PhotonNetwork.LocalPlayer.ActorNumber]);
        //}
        //else//受信.
        //{
        //    ID = (List<int>)stream.ReceiveNext();
        //    score = (List<int>)stream.ReceiveNext();
        //}
    }
}
