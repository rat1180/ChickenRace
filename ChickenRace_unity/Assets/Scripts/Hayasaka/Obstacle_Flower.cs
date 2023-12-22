using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Flower : Obstacle
{
    [SerializeField]
    bool isFlowerLeftPaunchFlg;
    [SerializeField]
    bool isFlowerRightPaunchFlg;
    [SerializeField]
    bool isFlowerPaunchWaitFlg;

    [SerializeField]
    GameObject paunch;
    [SerializeField]
    GameObject paunchTargetLeft;
    [SerializeField]
    GameObject paunchTargetRight;

    [SerializeField]
    Vector2 thisPos;
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        isFlowerLeftPaunchFlg = false;
        isFlowerRightPaunchFlg = false;
        isFlowerPaunchWaitFlg = false;
    }
    protected override void update()
    {
        thisPos = this.gameObject.transform.position;
        if (GameManager.instance.CheckObstacleMove())
        {
            if (isFlowerLeftPaunchFlg)
            {
                ShotObj();
            }
            if (isFlowerRightPaunchFlg)
            {
                ShotObj();
            }
        }
    }
    protected override void ObjStart()
    {
        
    }
    /// <summary>
    /// 角度取得から拳生成
    /// </summary>
    void ShotObj()
    {
        isFlowerLeftPaunchFlg = false;
        Invoke("WaitFlg", 6.0f);
    }
    void WaitFlg()
    {
        isFlowerPaunchWaitFlg = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var toucheObj = other.gameObject.transform.position;
        //パンチが戻るまで判定しない
        if (!isFlowerPaunchWaitFlg)
        {
            if (thisPos.x > toucheObj.x)
            {
                isFlowerLeftPaunchFlg = true;
            }
            else
            {
                isFlowerRightPaunchFlg = true;
            }
        }
    }
    
}
