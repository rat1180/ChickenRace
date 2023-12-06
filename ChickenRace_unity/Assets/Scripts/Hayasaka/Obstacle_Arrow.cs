using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Arrow : Obstacle
{
    [SerializeField]
    float ShotCnt;

    [SerializeField]
    bool ShotFlg;

    /// <summary>
    /// èâä˙âª
    /// </summary>
    public override void Init()
    {
        ObstacleCenterPos = new Vector2Int(0, 0);
        ShotCnt = 3.0f;
        ShotFlg = false;
    }
    public override void update()
    {
        ShotCnt -= Time.deltaTime;
        if (ShotCnt < 0.0f)
        {
            ShotFlg = true;
        }
    }
    public override void ObjStart()
    {
        if(ShotFlg)
        {
            ShotObj();
        }
    }
    void ShotObj()
    {
        Debug.Log("î≠éÀ");
    }
}
