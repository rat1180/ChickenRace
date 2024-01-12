using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Death : Obstacle
{
    /// <summary>
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    protected override void update()
    {
     
    }
    protected override void ObjStart()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //Destroy(other.gameObject);
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        //Destroy(other.gameObject);
    }
}
