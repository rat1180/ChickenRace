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
    /// �v���C���[�̒Ǐ].
    /// </summary>
    private void TargetMove()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, 10.0f * Time.fixedDeltaTime);
    }

    /// <summary>
    /// �v���C���[������W���擾.
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
            // �v���C���[�I�u�W�F�N�g�̈ʒu���𑗐M
            stream.SendNext(targetPos);
        }
        else
        {
            // �v���C���[�I�u�W�F�N�g�̈ʒu������M
            targetPos = (Vector3)stream.ReceiveNext();
        }
    }
}