using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Door : Obstacle
{
    [SerializeField]
    float colCnt;

    [SerializeField]
    bool isColFlg;

    BoxCollider2D cbc2d;
    BoxCollider2D cbc2d2;

    GameObject ChildObj;
    GameObject ChildObj2;
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        colCnt = 3.0f;
        isColFlg = true;

        ChildObj = gameObject.transform.GetChild(0).gameObject;
        ChildObj2 = gameObject.transform.GetChild(1).gameObject;

        cbc2d = ChildObj.GetComponent<BoxCollider2D>();
        cbc2d2 = ChildObj2.GetComponent<BoxCollider2D>();
    }
    protected override void update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            if (isColFlg)
            {
                if (colCnt > 0.0f)
                {
                    colCnt -= Time.deltaTime;
                }
                if (colCnt < 0.0f)
                {
                    ChangeActive();
                    colCnt = 3.0f;
                }
            }
        }
    }
    protected override void ObjStart()
    {
        colCnt = 3.0f;
        isColFlg = true;
    }
    /// <summary>
    /// オブジェクトの表示非表示を繰り返す
    /// </summary>
    void ChangeActive()
    {
        if (cbc2d.enabled && cbc2d2.enabled)
        {
            cbc2d.enabled = false;
            cbc2d2.enabled = false;
        }
        else
        {
            cbc2d.enabled = true;
            cbc2d2.enabled = true;
        }
    }
}
