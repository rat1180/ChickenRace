using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Surprise : Obstacle
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
    void OnCollsionEnter2D(Collision2D other)
    {
        
    }
}
