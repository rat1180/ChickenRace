using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Surprise : Obstacle
{
    [SerializeField]
    bool moveFlg;
    
    float speed;
    int counter;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        moveFlg = false;
        speed = 0.05f;
        counter = 0;
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    protected override void update()
    {
        if (moveFlg)
        {
            this.transform.Translate(new Vector3(0, speed, 0));
            counter++;
            if (counter == 100)
            {
                moveFlg = false;
            }
        }
    }
    
    protected override void ObjStart()
    {
       
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        moveFlg = true;
    }
}
