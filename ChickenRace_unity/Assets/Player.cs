using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{ 
    Rigidbody2D rb;
    
    [SerializeField] Vector3 moveVector;
    [SerializeField] float moveSpeed; // �v���C���[�̈ړ��X�s�[�h.
    [SerializeField] bool isJump;     // �W�����v�ł��邩�ǂ���.
    [SerializeField] float addJump;   // �W�����v��.
    [SerializeField] float rayDis;    // ���C�̒���.

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector3(moveVector.x * moveSpeed * Time.deltaTime, rb.velocity.y, 0);
        
        // �f�o�b�O�p.
        //rb.velocity = new Vector3(moveVector.x, moveVector.y, 0) * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isJump = true;
        DirectionCheck(collision.contacts[0].point);
    }

    /// <summary>
    /// �v���C���[�̈ړ�.
    /// </summary>
    private void OnMove(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        moveVector = new Vector3(velocity.x, velocity.y, 0);
    }

    /// <summary>
    /// �v���C���[�̃W�����v����.
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
        }
        else if (angle < 135 - threshold)
        {
            //Debug.Log("��");
        }
        else if (angle < 225 - threshold)
        {
            //Debug.Log("��");
        }
        else if (angle < 315 - threshold)
        {
            //Debug.Log("��");
        }
    }
}
