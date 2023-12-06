using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    Vector3 targetPos;
    
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        TargetMove();
    }

    /// <summary>
    /// プレイヤーの追従.
    /// </summary>
    private void TargetMove()
    {   
        transform.position = Vector3.Lerp(transform.position, targetPos, 1.0f);
    }

    /// <summary>
    /// プレイヤーから座標を取得.
    /// </summary>
    /// <param name="TargetPos"></param>
    public void PositionUpdate(Vector3 targetpos)
    {
        targetPos = targetpos;
    }
}
