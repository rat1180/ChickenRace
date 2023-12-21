using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun, IPunObservable
{
    public Vector3 targetPos;
    GameObject target;
    
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

    /// <summary>
    /// ���g���폜.
    /// </summary>
    private void myDestroy()
    {
        if(target == null)
        {
            Destroy(gameObject);
        }
    }
}