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
    /// ボールを射出する
    /// </summary>
    void ThrowingArrow()
    {
            // 射出角度
            float angle = throwingAngle;

            // 射出速度を算出
            Vector3 velocity = CalculateVelocity(this.transform.position, eend, angle);

            // 射出
            
            rrb.AddForce(velocity * rb.mass, ForceMode2D.Impulse);
        
       
    }

    /// <summary>
    /// 標的に命中する射出速度の計算
    Vector3 CalculateVelocity(Vector3 pointA, Vector3 pointB, float angle)
    {
        // 射出角をラジアンに変換
        float rad = angle * Mathf.PI / 180;

        // 水平方向の距離x
        float x = Vector2.Distance(new Vector2(pointA.x, pointA.z), new Vector2(pointB.x, pointB.z));

        // 垂直方向の距離y
        float y = pointA.y - pointB.y;

        // 斜方投射の公式を初速度について解く
        float speed = Mathf.Sqrt(-Physics.gravity.y * Mathf.Pow(x, 2) / (2 * Mathf.Pow(Mathf.Cos(rad), 2) * (x * Mathf.Tan(rad) + y)));

        if (float.IsNaN(speed))
        {
            // 条件を満たす初速を算出できなければVector3.zeroを返す
            return Vector3.zero;
        }
        else
        {
            return (new Vector3(pointB.x - pointA.x, x * Mathf.Tan(rad), pointB.z - pointA.z).normalized * speed);
        }
    }
}
