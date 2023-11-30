using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{ 
    Rigidbody2D rb;
    
    [SerializeField] Vector3 moveVector;
    [SerializeField] float moveSpeed; // プレイヤーの移動スピード.
    [SerializeField] bool isJump;     // ジャンプできるかどうか.
    [SerializeField] float addJump;   // ジャンプ力.
    [SerializeField] float rayDis;    // レイの長さ.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveVector.x * moveSpeed * Time.deltaTime, rb.velocity.y, 0);
        
        // デバッグ用.
        //rb.velocity = new Vector3(moveVector.x, moveVector.y, 0) * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJump = true;
        DirectionCheck(collision.contacts[0].point);
    }

    /// <summary>
    /// プレイヤーの移動.
    /// </summary>
    private void OnMove(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        moveVector = new Vector3(velocity.x, velocity.y, 0);
    }

    /// <summary>
    /// プレイヤーのジャンプ処理.
    /// </summary>
    private void OnJump()
    {
        if(isJump == true)
        {
            // Debug.Log("jump");
            rb.AddForce(transform.up * addJump, ForceMode2D.Impulse);
            isJump = false;
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
        }
        else if (angle < 135 - threshold)
        {
            //Debug.Log("上");
        }
        else if (angle < 225 - threshold)
        {
            //Debug.Log("左");
        }
        else if (angle < 315 - threshold)
        {
            //Debug.Log("下");
        }
    }
}
