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
        look = this.transform.rotation;
        other.GetComponent<Obstacle_ArrowShot>().BlowArrow(look, pow);
    }
}
