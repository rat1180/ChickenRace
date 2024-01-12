using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Cannon : Obstacle
{
    [SerializeField]
    Quaternion look;
    [SerializeField]
    float pow;
    /// <summary>
    /// ������
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        pow = 1.5f;
    }
    /// <summary>
    /// �ڐG���ɑ�C�̏�����ɓ]��������
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter2D(Collider2D other)
    {
        look = this.transform.rotation;
        other.GetComponent<Obstacle_ArrowShot>().BlowArrow(look, pow);
    }
}
