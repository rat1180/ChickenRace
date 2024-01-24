using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun, IPunObservable
{
    public Vector3 targetPos;
    public GameObject target;
    Vector3 savePos;
    Vector3 relativeVector;
    [SerializeField] float threshold; // アニメーション用閾値.
    public bool isTurnFlg = true;
    bool isDestroy;
    CharaAnimation.Animations nowAnim;

    void Start()
    {
        savePos = transform.position;
        isDestroy = false;
    }

    void FixedUpdate()
    {
        TargetMove();
        myDestroy();
        IsTurn();
    }

    /// <summary>
    /// プレイヤーの追従.
    /// </summary>
    private void TargetMove()
    {
        if (photonView.IsMine)
        {
            transform.position = targetPos;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, 10.0f * Time.fixedDeltaTime);
        }
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

    /// <summary>
    /// 自身を削除.
    /// </summary>
    public void myDestroy()
    {
        if((target == null && photonView.IsMine) || isDestroy)
        {
            photonView.RPC("DestroyRPC", RpcTarget.Others);
            Destroy(gameObject);
        }
    }

    [PunRPC]
    void DestroyRPC()
    {
        isDestroy = true;
    }

    public void SetAnim(CharaAnimation.Animations code)
    {
        photonView.RPC("SetAnimRPC", RpcTarget.Others, (int)code);
    }

    [PunRPC]
    void SetAnimRPC(int code)
    {
        GetComponent<CharaAnimation>().nowAnimations = (CharaAnimation.Animations)code;
    }

    /// <summary>
    /// 画像の向き反転：左<->右
    /// </summary>
    public void IsTurn()
    {
        if (!isTurnFlg) return;

        // 相対ベクター取得.
        relativeVector = transform.position - savePos;

        if(Mathf.Abs(relativeVector.x) < threshold)
        {
            relativeVector = Vector3.zero;
        }

        // 左に進んでいるとき.
        if (relativeVector.x < 0)
        {
            if(transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        // 右に進んでいるとき.
        else if(relativeVector.x > 0)
        {
            if (transform.localScale.x < 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }

        savePos = transform.position;

    }
}