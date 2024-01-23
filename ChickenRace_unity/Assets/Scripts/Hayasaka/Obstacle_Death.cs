using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Death : Obstacle
{
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
    }
    protected override void update()
    {
     
    }
    protected override void ObjStart()
    {
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            Destroy(gameObject);
        }
       
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
    void Destoroy()
    {
        Destroy(this.gameObject);
    }
}
