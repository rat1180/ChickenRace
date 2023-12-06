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
            if (shotCnt > 0.0f)
            {
                shotCnt -= Time.deltaTime;
            }
            if (shotCnt < 0.0f)
            {
                ShotObj();
                shotCnt = 3.0f;
            }
        }
    }
    public override void ObjStart()
    {
        shotCnt = 3.0f;
        isShotFlg = true;
    }
    void ShotObj()
    {
        Debug.Log("î≠éÀ");
    }
}
