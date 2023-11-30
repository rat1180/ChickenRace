using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]InputAction InputAction;
    Rigidbody2D Rb;

    Vector3 MoveVector;
    [SerializeField]float MoveSpeed; // プレイヤーの移動スピード.

    private void OnDisable()
    {
        // InputActionを無効にする.
        InputAction.Disable();
    }

    private void OnEnable()
    {
        // InputActionを有効にする.
        InputAction.Enable();
    }

    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMove();
    }

    /// <summary>
    /// プレイヤーの移動.
    /// </summary>
    void PlayerMove()
    {
        MoveVector = InputAction.ReadValue<Vector2>();
        Rb.velocity = new Vector3(MoveVector.x, MoveVector.y, 0) * MoveSpeed * Time.deltaTime;
    }
    
}
