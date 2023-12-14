using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_ParabolaArrowShot : Obstacle_ArrowShot
{
    [SerializeField]
    float throwingAngle;
    [SerializeField]
    Vector3 eend;
    [SerializeField]
    Vector3 sstart;
    [SerializeField]
    Rigidbody2D rrb;
    // Start is called before the first frame update
    void Start()
    {
        rrb = this.gameObject.GetComponent<Rigidbody2D>();

        throwingAngle = 45.0f;
        sstart = this.transform.position;
        eend = sstart;
        eend.x += 5.0f;
        eend.y += 5.0f;
        speed = 1.5f;
        //ThrowingArrow();
    }

    // Update is called once per frame
    void Update()
    {
        ThrowingArrow();
    }
    /// <summary>
    /// �{�[�����ˏo����
    /// </summary>
    void ThrowingArrow()
    {
            // �ˏo�p�x
            float angle = throwingAngle;

            // �ˏo���x���Z�o
            Vector3 velocity = CalculateVelocity(this.transform.position, eend, angle);

            // �ˏo
            
            rrb.AddForce(velocity * rb.mass, ForceMode2D.Impulse);
        
       
    }

    /// <summary>
    /// �W�I�ɖ�������ˏo���x�̌v�Z
    Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // �ˏo�p�����W�A���ɕϊ�
        float rad = angle * Mathf.PI / 180;

        // ���������̋���x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // ���������̋���y
        float y = pointA.y - pointB.y;

        // �Ε����˂̌����������x�ɂ��ĉ���
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // �����𖞂����������Z�o�ł��Ȃ����Vector3.zero��Ԃ�
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }
}
