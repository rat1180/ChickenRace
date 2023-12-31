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
    public float elapsedTime;//経過時間.

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.SetDataSheringClass(gameObject.GetComponent<DataSharingClass>());
        //人数分スコアの入れ物を用意する.
        for(int i=0;i< ConectServer.RoomProperties.MaxPlayer; i++)
        {
            score.Add(0);
            rankTime.Add(0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))//デバッグ用
        {
            //1人目が0番目になるように-1(RPC内でActorNumberを引数にすると全員自分を呼んでしまうため代入).
            int number = PhotonNetwork.LocalPlayer.ActorNumber - 1;
            //photonView.RPC(nameof(PushGoalTime), RpcTarget.All,number,timer);
            photonView.RPC(nameof(PushGoalTime), RpcTarget.All, number, 1);
        }
    }

    /// <summary>
    /// 外部からIDを受け取ってリストに入れる関数
    /// 引数にはIDを1つずつ指定(リストはサポートされていない).
    /// </summary>
    public void PushID(int iD)
    {
        photonView.RPC(nameof(PushIDRPC), RpcTarget.All, iD);
    }

    /// <summary>
    /// IDを入れるRPC.
    /// </summary>
    [PunRPC]
    private void PushIDRPC(int iD)
    {
        ID.Add(iD);
    }

    /// <summary>
    /// 外部からindexを受け取って指定された場所を0にする関数
    /// 引数にはindexを1つずつ指定.
    /// </summary>
    public void ResetID(int index)
    {
        photonView.RPC(nameof(ResetIDRPC), RpcTarget.All, index);
    }

    /// <summary>
    /// 指定されたindexを0にするRPC.
    /// </summary>
    [PunRPC]
    private void ResetIDRPC(int index)
    {
        ID[index] = 0;
    }

    /// <summary>
    /// 外部からリストを初期化する関数.
    /// </summary>
    public void ResetIDList()
    {
        photonView.RPC(nameof(ResetIDListRPC), RpcTarget.All);
    }

    /// <summary>
    /// リストを初期化するRPC.
    /// </summary>
    [PunRPC]
    private void ResetIDListRPC()
    {
        ID = new List<int>();
    }

    /// <summary>
    /// 外部から順位(Rank)を受け取ってリストに入れる関数
    /// 引数には自分のアクターナンバー,順位を指定.
    /// </summary>
    public void PushScore(int index,int myscore)
    {
        photonView.RPC(nameof(PushScoreRPC), RpcTarget.All,index, myscore);
    }

    /// <summary>
    /// 順位を入れるRPC.
    /// </summary>
    [PunRPC]
    private void PushScoreRPC(int index,int myscore)
    {
        score[index] += myscore;
    }


    /// <summary>
    /// 経過時間を入れる関数
    /// </summary>
    public void PushElapsedTime(float time)
    {
        photonView.RPC(nameof(PushElapsedTimeRPC), RpcTarget.All, time);
    }

    /// <summary>
    /// 指定されたindexを0にするRPC.
    /// </summary>
    [PunRPC]
    private void PushElapsedTimeRPC(float time)
    {
        elapsedTime = time;
    }

    /// <summary>
    /// 外部からindexを受け取って指定された場所を0にする関数
    /// 引数にはindexを1つずつ指定.
    /// </summary>
    public float ReturnTime(int index)
    {
        return rankTime[index];
    }

    /// <summary>
    /// 指定されたindexの値を返すRPC.
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
    /// ゴールしたタイミングで関数を呼び出す
    /// </summary>
    [PunRPC]
    void PushGoalTimeRPC(int index,float time)
    {
        //score[number] += point;
        rankTime[index] = time; 
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
