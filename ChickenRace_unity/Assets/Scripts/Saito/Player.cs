
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
    Gamepad gamepad;

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
    [SerializeField] bool isCoroutine;

    /// <summary>
    /// 初期化用関数.
    /// </summary>
    private void Init()
    {
        rb = GetComponent<Rigidbody2D>();
        
        nowHitDir = HitDirList.NONE;
        charaAnimation.nowAnimations = CharaAnimation.Animations.IDLE;
        rb.isKinematic = true; // 重力の停止.
    }

    private void OnDisable()
    {
        
    }

    void Start()
    {
        Init();
    }

    void FixedUpdate()
    {
        if (gamepad == null)
        {
            gamepad = Gamepad.current; // ゲームパッドの取得.
            
        }
        PlayerDeath();
        if(isStart && !isGoal && !isDeath)
        {
            PlayerMove();
            WallSliding();
            rb.isKinematic = false; // 重力の開始.
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
            if (!isGoal)
            {
                isGoal = true;
                SoundManager.instance.PlaySE(SoundName.SECode.SE_Goal_Voice);
                SoundManager.instance.PlaySE(SoundName.SECode.SE_Goal);
            }
        }
        else if(collision.gameObject.tag == "Damage")
        {
            if (!isDeath)
            {
                SoundManager.instance.PlaySE(SoundName.SECode.SE_Damage);

                System.Action waitAction = () =>
                {
                    if(gamepad != null)
                    {
                        gamepad.SetMotorSpeeds(0, 0);
                    }
                    
                    // プレイヤー死亡処理.
                    isDeath = true;
                };

                if (isCoroutine == false)
                {
                    if(gamepad != null)
                    {
                        gamepad.SetMotorSpeeds(1.0f, 1.0f);
                    }
                    
                    charaAnimation.nowAnimations = CharaAnimation.Animations.DEATH;
                    // 指定した秒数の後にwaitActionを実行.
                    StartCoroutine(WaitTime(2.0f, waitAction));
                    isCoroutine = true;
                }
               
            }
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
            SoundManager.instance.PlaySE(SoundName.SECode.SE_Jump);

            System.Action waitAction = () =>
            {
                if(gamepad != null)
                {
                    gamepad.SetMotorSpeeds(0, 0);
                }
                
            };

            if (isCoroutine == false)
            {
                if(gamepad != null)
                {
                    gamepad.SetMotorSpeeds(0.1f, 0.1f);
                }
                
                // 指定した秒数の後にwaitActionを実行.
                StartCoroutine(WaitTime(0.1f, waitAction));
                isCoroutine = true;
            }
        }
    }

    /// <summary>
    /// 3秒指定したボタン・キーが押されたときに呼ばれる.
    /// </summary>
    void OnGiveUp()
    {
        Debug.Log("死亡");
        SoundManager.instance.PlaySE(SoundName.SECode.SE_Damage);

        System.Action waitAction = () =>
        {
            if(gamepad != null)
            {
                gamepad.SetMotorSpeeds(0, 0);
            }
            
            
            // プレイヤー死亡処理.
            isDeath = true;
        };

        if (isCoroutine == false)
        {
            if(gamepad != null)
            {
                gamepad.SetMotorSpeeds(1.0f, 1.0f);
            }
            
            charaAnimation.nowAnimations = CharaAnimation.Animations.DEATH;
            // 指定した秒数の後にwaitActionを実行.
            StartCoroutine(WaitTime(2.0f, waitAction));
            isCoroutine = true;
        }

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

        // 計算で出た角度が-180〜180なため、0〜360に変換.
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
        instanceObj.GetComponent<Character>().target = this.gameObject;
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
        instanceObj.GetComponent<Character>().myDestroy();
        Destroy(gameObject);
    }

    /// <summary>
    /// プレイヤーがやられたときに呼ぶ関数.
    /// </summary>
    void PlayerDeath()
    {
        if (isDeath)
        {
            GameManager.instance.DeadPlayer();
        }
    }

    /// <summary>
    /// 指定した秒数待つ処理.
    /// </summary>
    /// <param name="time"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator WaitTime(float time, System.Action action)
    {
        yield return new WaitForSeconds(time);
        Debug.Log("コルーチン呼ぶ");
        action.Invoke();
        isCoroutine = false;
    }
}
