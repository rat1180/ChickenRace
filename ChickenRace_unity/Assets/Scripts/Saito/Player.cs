
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using ConstList;
using Photon.Pun;
using PhotonMethods;

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
    [SerializeField] GameObject playerImage; // �v���C���[�̉摜.
    [SerializeField] GameObject instanceObj; // ���������I�u�W�F�N�g.
    [SerializeField] bool isGoal;

    /// <summary>
    /// �������p�֐�.
    /// </summary>
    private void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        nowHitDir = HitDirList.NONE;
        nowPlayerAction = PlayerAction.NONE;
        ImageInstance();
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        PlayerMove();
        WallSliding();
        PlayerTransform();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        DirectionCheck(collision.contacts[0].point);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        nowHitDir = HitDirList.NONE;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Goal")
        {
            isGoal = true;
        }
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
        //CharaAnimation.Instance.StartAnimation(CharaAnimation.Animations.MOVE);
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

    /// <summary>
    /// �摜�̐���.
    /// </summary>
    private void ImageInstance()
    {
        instanceObj = "CharAnimObj".SafeInstantiate(transform.position, transform.rotation);
    }

    /// <summary>
    /// �v���C���[�̍��W���Z�b�g.
    /// </summary>
    private void PlayerTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }

    /// <summary>
    /// �S�[���������𑗂�֐�.
    /// </summary>
    /// <returns></returns>
    public bool GoalCheck()
    {
        return isGoal;
    }
}
