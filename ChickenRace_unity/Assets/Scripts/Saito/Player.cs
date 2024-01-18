
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
    [SerializeField] bool isMove;           // 動いているかどうか.
    [SerializeField] float moveSpeed;       // プレイヤーの移動スピード.
    [SerializeField] float minMoveSpeed;    // 最小移動力.
    [SerializeField] float maxMoveSpeed;    // 最大移動力.
    [SerializeField] float moveDecay;       // 移動減衰値.
    [SerializeField] float jumpPower;       // ジャンプ力.
    [SerializeField] Vector2 wallJumpPower; // 壁キックの力.
    [SerializeField] float wallSlidingSpeed;// 壁滑りのスピード.
    [SerializeField] HitDirList nowHitDir;  // プレイヤーがオブジェクトとどの向きで衝突したか.
    [SerializeField] GameObject instanceObj; // 生成したオブジェクト.
    [SerializeField] bool isStart;           // ゲームがスタートしたかの判定.
    [SerializeField] bool isGoal;            // プレイヤーがゴールしたかの判定.
    [SerializeField] bool isDeath;           // プレイヤーが死亡したかの判定.
    [SerializeField] float time;

    /// <summary>
    /// 初期化用関数.
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
            // プレイヤー死亡処理.
            isDeath = true;
        }
    }

    /// <summary>
    /// プレイヤーの移動処理.
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
    /// PlayerInputのMoveからキー情報や値の受け取り.
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
    /// プレイヤーのジャンプ処理.
    /// PlayerInputのJumpからキー情報や値の受け取り.
    /// </summary>
    private void OnJump()
    {
        // 既に空中にいるときはジャンプアニメーションを再生させない.
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
    /// 3秒指定したボタン・キーが押されたときに呼ばれる.
    /// </summary>
    void OnGiveUp()
    {
        Debug.Log("死亡");
        PlayerDeath();
    }

    /// <summary>
    /// 壁滑り処理.
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
    /// オブジェクトと衝突したときどこが衝突したかのを取得.
    /// </summary>
    /// <param name="hitpoint"></param>
    void DirectionCheck(Vector3 hitpoint)
    {
        // 閾値.
        var threshold = 10;

        // 衝突したオブジェクトとの相対ベクター取得.
        Vector3 relativeVector = (hitpoint - transform.position).normalized;
        float angle = Mathf.Atan2(relativeVector.y, relativeVector.x) * Mathf.Rad2Deg;

        // 計算で出た角度が-180～180なため、0～360に変換.
        if (angle < 0)
        {
            angle += 360;
        }

        // Debug.Log(angle);

        if (angle >= 315 - threshold)
        {
            //Debug.Log("右");
            nowHitDir = HitDirList.HIT_RIGHT;
        }
        else if (angle < 135 - threshold)
        {
            //Debug.Log("上");
            nowHitDir = HitDirList.HIT_UP;
        }
        else if (angle < 225 - threshold)
        {
            //Debug.Log("左");
            nowHitDir = HitDirList.HIT_LEFT;
        }
        else if (angle < 315 - threshold)
        {
            //Debug.Log("下");
            nowHitDir = HitDirList.HIT_DOWN;
        }
    }

    /// <summary>
    /// 画像の生成.
    /// </summary>
    public void ImageInstance()
    {
        instanceObj = "CharAnimObj".SafeInstantiate(transform.position, transform.rotation);
        charaAnimation = instanceObj.GetComponent<CharaAnimation>();
    }

    /// <summary>
    /// プレイヤーの座標をセット.
    /// </summary>
    private void PlayerTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }

    /// <summary>
    /// ゲームが開始したかを判定.
    /// </summary>
    /// <returns></returns>
    public void IsStart(bool isstart)
    {
        isStart = isstart;
    }

    /// <summary>
    /// ゴールしたかを送る関数.
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
    /// 自身を削除.
    /// </summary>
    public void PlayerDelete()
    {
        Destroy(instanceObj);
        Destroy(gameObject);
    }

    /// <summary>
    /// プレイヤーがやられたときに呼ぶ関数.
    /// </summary>
    void PlayerDeath()
    {
        if (isDeath)
        {
            charaAnimation.nowAnimations = CharaAnimation.Animations.DEATH;
        }
    }
}
