using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Arrow : Obstacle
{
    [SerializeField]
    float shotCnt;

    [SerializeField]
    bool isShotFlg;

    /// <summary>
    /// èâä˙âª
    /// </summary>
    public override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        shotCnt = 3.0f;
        isShotFlg = true;
    }
    public override void update()
    {
        if (isShotFlg)
        {
            ObjStart();
        }
    }
    public override void ObjStart()
    {
        if (shotCnt > 0.0f)
        {
            shotCnt -= Time.deltaTime;
        }
        if (shotCnt < 0.0f)
        {
            ShotObj();
        }
    }
    void ShotObj()
    {
        isShotFlg = false;
        Debug.Log("î≠éÀ");
    }
}
