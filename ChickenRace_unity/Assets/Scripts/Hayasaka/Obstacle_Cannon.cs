using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Cannon : Obstacle
{
    Vector2 look;
    float pow;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    public override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        look = new Vector3(0, 0,1);
        pow = 1.5f;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        other.GetComponent<Obstacle_ArrowShot>().BlowArrow(look, pow);
    }
}
