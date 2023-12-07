using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Character : MonoBehaviourPun, IPunObservable
{
    Vector3 targetPos;

    void Start()
    {

    }

    void FixedUpdate()
    {
        TargetMove();
    }

    /// <summary>
    /// プレイヤーの追従.
    /// </summary>
    private void TargetMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 10.0f * Time.fixedDeltaTime);
    }

    /// <summary>
    /// プレイヤーから座標を取得.
    /// </summary>
    /// <param name="TargetPos"></param>
    public void PositionUpdate(Vector3 targetpos)
    {
        targetPos = targetpos;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // プレイヤーオブジェクトの位置情報を送信
            stream.SendNext(targetPos);
        }
        else
        {
            // プレイヤーオブジェクトの位置情報を受信
            targetPos = (Vector3)stream.ReceiveNext();
        }
    }
}