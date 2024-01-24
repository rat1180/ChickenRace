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
    [SerializeField] float threshold; // �A�j���[�V�����p臒l.
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
    /// �v���C���[�̒Ǐ].
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
    /// �摜�̌������]�F��<->�E
    /// </summary>
    public void IsTurn()
    {
        if (!isTurnFlg) return;

        // ���΃x�N�^�[�擾.
        relativeVector = transform.position - savePos;

        if(Mathf.Abs(relativeVector.x) < threshold)
        {
            relativeVector = Vector3.zero;
        }

        // ���ɐi��ł���Ƃ�.
        if (relativeVector.x < 0)
        {
            if(transform.localScale.x > 0)
            {
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
            }
        }
        // �E�ɐi��ł���Ƃ�.
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