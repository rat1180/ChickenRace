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
    /// �v���C���[�̒Ǐ].
    /// </summary>
    private void TargetMove()
    {   
        transform.position = Vector3.Lerp(transform.position, targetPos, 1.0f);
    }

    /// <summary>
    /// �v���C���[������W���擾.
    /// </summary>
    /// <param name="TargetPos"></param>
    public void PositionUpdate(Vector3 targetpos)
    {
        targetPos = targetpos;
    }
}
