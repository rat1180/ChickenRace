using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{ 
    Rigidbody2D Rb;
    [SerializeField] Vector3 MoveVector;
    [SerializeField] bool isMove;
    [SerializeField] float MoveSpeed; // プレイヤーの移動スピード.
    [SerializeField] bool isJump;     // ジャンプできるかどうか.
    [SerializeField] float AddJump;   // ジャンプ力.

    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Rb.velocity = new Vector3(MoveVector.x * MoveSpeed * Time.deltaTime, Rb.velocity.y, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJump = true;
    }

    /// <summary>
    /// プレイヤーの移動.
    /// </summary>
    private void OnMove(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        MoveVector = new Vector3(velocity.x, 0, 0);
    }

    private void OnJump()
    {
        if(isJump == true)
        {
            Debug.Log("jump");
            Rb.AddForce(transform.up * AddJump, ForceMode2D.Impulse);
            isJump = false;
        }
    }
}
