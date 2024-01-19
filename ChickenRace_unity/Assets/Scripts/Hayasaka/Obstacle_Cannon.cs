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
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        pow = 1.5f;
    }
    /// <summary>
    /// ê⁄êGéûÇ…ëÂñCÇÃè„ï˚å¸Ç…ì]ä∑Ç≥ÇπÇÈ
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            look = this.transform.rotation;

            other.GetComponent<Rigidbody2D>().AddForce(transform.up * (pow * 30), ForceMode2D.Impulse);
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
