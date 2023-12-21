using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Move : Obstacle
{
    [SerializeField]
    Vector3 pos;
    public GameObject RF;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        pos = this.transform.position;
    }
    protected override void update()
    {
        //MoveBeside();
        //MoveVertical();
        //MoveDiagonal();
        MoveRotate();
    }
    void MoveBeside()
    {
        transform.position = new Vector3(pos.x + Mathf.PingPong(Time.time, 5), pos.y, pos.z);
    }
    void MoveVertical()
    {
        transform.position = new Vector3(pos.x, pos.y + Mathf.PingPong(Time.time, 5), pos.z);
    }
    void MoveDiagonal()
    {
        transform.position = new Vector3(pos.x + Mathf.PingPong(Time.time, 5), pos.y + Mathf.PingPong(Time.time, 5), pos.z);
    }
    void MoveRotate()
    {
        RF.transform.Rotate(new Vector3(0, 0, 0.2f));
    }
    protected override void ObjStart()
    {
       
    }
    
}
