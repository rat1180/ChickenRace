using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Cannon : Obstacle
{
    [SerializeField]
    Quaternion look;
    [SerializeField]
    float pow;

    void Update()
    {
        if (GameManager.instance.CheckRaceEnd())
        {
            Destoroy();
        }
    }
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        pow = 1.5f;
    }
    /// <summary>
    /// 接触時に大砲の上方向に転換させる
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            look = this.transform.rotation;

            other.GetComponent<Rigidbody2D>().AddForce(transform.up * (pow * 15), ForceMode2D.Impulse);
            return;
        }


        look = this.transform.rotation;
        other.GetComponent<Obstacle_ArrowShot>().BlowArrow(look, pow);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    void Destoroy()
    {
        Destroy(this.gameObject);
    }
}
