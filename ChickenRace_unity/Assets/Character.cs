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

    private void TargetMove()
    {
        Vector3 CharaPos = targetPos - transform.position;
        transform.position = CharaPos;
    }

    public void PositionUpdate(Vector3 TargetPos)
    {
        targetPos = TargetPos;
    }
}
