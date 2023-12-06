
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ConstList;

public class Player : MonoBehaviour
{ 
    Rigidbody2D rb;
    
    [SerializeField] Vector3 moveVector;
    [SerializeField] bool isMove;           // �����Ă��邩�ǂ���.
    [SerializeField] float moveSpeed;       // �v���C���[�̈ړ��X�s�[�h.
    [SerializeField] float minMoveSpeed;    // �ŏ��ړ���.
    [SerializeField] float maxMoveSpeed;    // �ő�ړ���.
    [SerializeField] float moveDecay;       // �ړ������l.
    [SerializeField] float jumpPower;       // �W�����v��.
    [SerializeField] Vector2 wallJumpPower; // �ǃL�b�N�̗�.
    [SerializeField] float wallSlidingSpeed;// �Ǌ���̃X�s�[�h.
    [SerializeField] HitDirList nowHitDir;  // �v���C���[���I�u�W�F�N�g�Ƃǂ̌����ŏՓ˂�����.
    [SerializeField] PlayerAction nowPlayerAction;  // �v���C���[�����̍s�������Ă��邩.

    /// <summary>
    /// �������p�֐�.
    /// </summary>
    private void Init()
    {
        nowHitDir = HitDirList.NONE;
        nowPlayerAction = PlayerAction.NONE;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Init();
    }

    void FixedUpdate()
    {
        PlayerMove();
        WallSliding();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DirectionCheck(collision.contacts[0].point);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        nowHitDir = HitDirList.NONE;
    }

    /// <summary>
    /// �v���C���[�̈ړ�����.
    /// </summary>
    void PlayerMove()
    {
        if (Mathf.Abs(rb.velocity.x) < maxMoveSpeed)
        {
            nowPlayerAction = PlayerAction.MOVE;
            rb.velocity = new Vector3(rb.velocity.x + (moveVector.x * moveSpeed * Time.deltaTime), rb.velocity.y, 0);
        }
        else if (Mathf.Abs(rb.velocity.x) < minMoveSpeed)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }

        rb.velocity = new Vector3(rb.velocity.x * moveDecay, rb.velocity.y, 0);

    }

    /// <summary>
    /// PlayerInput��Move����L�[����l�̎󂯎��.
    /// </summary>
    private void OnMove(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        moveVector = new Vector3(velocity.x, velocity.y, 0);
    }

    /// <summary>
    /// �v���C���[�̃W�����v����.
    /// PlayerInput��Jump����L�[����l�̎󂯎��.
    /// </summary>
    private void OnJump()
    {
        if(nowHitDir != HitDirList.NONE)
        {
            nowPlayerAction = PlayerAction.WALLJUMP;
            switch (nowHitDir)
            {
                case HitDirList.HIT_RIGHT:
                    rb.AddForce(new Vector2(-wallJumpPower.x, wallJumpPower.y) * jumpPower, ForceMode2D.Impulse);
                    nowPlayerAction = PlayerAction.WALLJUMP;
                    break;
                case HitDirList.HIT_LEFT:
                    rb.AddForce(new Vector2(wallJumpPower.x, wallJumpPower.y) * jumpPower, ForceMode2D.Impulse);
                    nowPlayerAction = PlayerAction.WALLJUMP;
                    break;
                case HitDirList.HIT_DOWN:
                    rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                    nowPlayerAction = PlayerAction.JUMP;
                    break;
            }
        }
    }

    /// <summary>
    /// �Ǌ��菈��.
    /// </summary>
    private void WallSliding()
    {
        switch (nowHitDir)
        {
            case HitDirList.HIT_RIGHT:
            case HitDirList.HIT_LEFT:
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - wallSlidingSpeed * Time.deltaTime, 0);
                break;
        }
    }

    /// <summary>
    /// �I�u�W�F�N�g�ƏՓ˂����Ƃ��ǂ����Փ˂������̂��擾.
    /// </summary>
    /// <param name="hitpoint"></param>
    void DirectionCheck(Vector3 hitpoint)
    {
        // 臒l.
        var threshold = 10;

        // �Փ˂����I�u�W�F�N�g�Ƃ̑��΃x�N�^�[�擾.
        Vector3 relativeVector = (hitpoint - transform.position).normalized;
        float angle = Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg;

        // �v�Z�ŏo���p�x��-180�`180�Ȃ��߁A0�`360�ɕϊ�.
        if (angle < 0)
        {
            angle += 360;
        }

        // Debug.Log(angle);

        if (angle >= 315 - threshold)
        {
            //Debug.Log("�E");
            nowHitDir = HitDirList.HIT_RIGHT;
        }
        else if (angle < 135 - threshold)
        {
            //Debug.Log("��");
            nowHitDir = HitDirList.HIT_UP;
        }
        else if (angle < 225 - threshold)
        {
            //Debug.Log("��");
            nowHitDir = HitDirList.HIT_LEFT;
        }
        else if (angle < 315 - threshold)
        {
            //Debug.Log("��");
            nowHitDir = HitDirList.HIT_DOWN;
        }
    }

    /// <summary>
    /// �v���C���[�̍s����Ԃ�����.
    /// </summary>
    /// <returns></returns>
    public PlayerAction GetPlayerAction()
    {
        return nowPlayerAction;
    }
}