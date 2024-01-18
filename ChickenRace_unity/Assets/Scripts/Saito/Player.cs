
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
    [SerializeField] CharaAnimation charaAnimation;

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
    [SerializeField] GameObject instanceObj; // ���������I�u�W�F�N�g.
    [SerializeField] bool isStart;           // �Q�[�����X�^�[�g�������̔���.
    [SerializeField] bool isGoal;            // �v���C���[���S�[���������̔���.
    [SerializeField] bool isDeath;           // �v���C���[�����S�������̔���.
    [SerializeField] float time;

    /// <summary>
    /// �������p�֐�.
    /// </summary>
    private void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        nowHitDir = HitDirList.NONE;
        charaAnimation.nowAnimations = CharaAnimation.Animations.IDLE;
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        if(isStart && !isGoal && !isDeath)
        {
            PlayerMove();
            WallSliding();
        }
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
        else if(collision.gameObject.tag == "Damage")
        {
            // �v���C���[���S����.
            isDeath = true;
        }
    }

    /// <summary>
    /// �v���C���[�̈ړ�����.
    /// </summary>
    void PlayerMove()
    {
        if (Mathf.Abs(rb.velocity.x) < maxMoveSpeed)
        {
            rb.velocity = new Vector3(rb.velocity.x + (moveVector.x * moveSpeed * Time.deltaTime), rb.velocity.y, 0);
        }
        if (Mathf.Abs(rb.velocity.x) < minMoveSpeed)
        {
            //Debug.Log(Mathf.Abs(rb.velocity.x));
            rb.velocity = new Vector3(0, rb.velocity.y, 0);

            if (nowHitDir == HitDirList.HIT_DOWN)
            {
                charaAnimation.nowAnimations = CharaAnimation.Animations.IDLE;
            }
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

        if (nowHitDir == HitDirList.HIT_DOWN)
        {
            charaAnimation.nowAnimations = CharaAnimation.Animations.MOVE;
        }
    }

    /// <summary>
    /// �v���C���[�̃W�����v����.
    /// PlayerInput��Jump����L�[����l�̎󂯎��.
    /// </summary>
    private void OnJump()
    {
        // ���ɋ󒆂ɂ���Ƃ��̓W�����v�A�j���[�V�������Đ������Ȃ�.
        if(nowHitDir != HitDirList.NONE)
        {
            charaAnimation.nowAnimations = CharaAnimation.Animations.JUMP;
            switch (nowHitDir)
            {
                case HitDirList.HIT_RIGHT:
                    rb.AddForce(new Vector2(-wallJumpPower.x, wallJumpPower.y) * jumpPower, ForceMode2D.Impulse);
                    break;
                case HitDirList.HIT_LEFT:
                    rb.AddForce(new Vector2(wallJumpPower.x, wallJumpPower.y) * jumpPower, ForceMode2D.Impulse);
                    break;
                case HitDirList.HIT_DOWN:
                    rb.AddForce(transform.up * jumpPower, ForceMode2D.Impulse);
                    break;
            }
        }
    }

    /// <summary>
    /// 3�b�w�肵���{�^���E�L�[�������ꂽ�Ƃ��ɌĂ΂��.
    /// </summary>
    void OnGiveUp()
    {
        Debug.Log("���S");
        PlayerDeath();
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

                charaAnimation.nowAnimations = CharaAnimation.Animations.WALLSLIDING;

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
    /// �摜�̐���.
    /// </summary>
    public void ImageInstance()
    {
        instanceObj = "CharAnimObj".SafeInstantiate(transform.position, transform.rotation);
        charaAnimation = instanceObj.GetComponent<CharaAnimation>();
    }

    /// <summary>
    /// �v���C���[�̍��W���Z�b�g.
    /// </summary>
    private void PlayerTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }

    /// <summary>
    /// �Q�[�����J�n�������𔻒�.
    /// </summary>
    /// <returns></returns>
    public void IsStart(bool isstart)
    {
        isStart = isstart;
    }

    /// <summary>
    /// �S�[���������𑗂�֐�.
    /// </summary>
    /// <returns></returns>
    public bool GoalCheck()
    {
        return isGoal;
    }

    public void StartPosition(Vector3 startpos)
    {
        transform.position = startpos;
    }

    /// <summary>
    /// ���g���폜.
    /// </summary>
    public void PlayerDelete()
    {
        Destroy(instanceObj);
        Destroy(gameObject);
    }

    /// <summary>
    /// �v���C���[�����ꂽ�Ƃ��ɌĂԊ֐�.
    /// </summary>
    void PlayerDeath()
    {
        if (isDeath)
        {
            charaAnimation.nowAnimations = CharaAnimation.Animations.DEATH;
        }
    }
}
