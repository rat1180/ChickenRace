using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField]InputAction InputAction;
    Rigidbody2D Rb;

    Vector3 MoveVector;
    [SerializeField]float MoveSpeed; // �v���C���[�̈ړ��X�s�[�h.

    private void OnDisable()
    {
        // InputAction�𖳌��ɂ���.
        InputAction.Disable();
    }

    private void OnEnable()
    {
        // InputAction��L���ɂ���.
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
    /// �v���C���[�̈ړ�.
    /// </summary>
    void PlayerMove()
    {
        MoveVector = InputAction.ReadValue<Vector2>();
        Rb.velocity = new Vector3(MoveVector.x, MoveVector.y, 0) * MoveSpeed * Time.deltaTime;
    }
    
}
