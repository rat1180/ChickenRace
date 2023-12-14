using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Tracking : Obstacle
{
    public GameObject trakingTarget;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    protected override void update()
    {
        Tracking();
    }
    void Tracking()
    {
        this.transform.position = trakingTarget.transform.position;
    }
    protected override void ObjStart()
    {
       
    }
    
}
