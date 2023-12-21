using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Tracking : MonoBehaviour
{
    public GameObject trakingTarget;
   
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            Tracking();
        }
    }
    /// <summary>
    /// 棒にくっつく
    /// </summary>
    void Tracking()
    {
        this.transform.position = trakingTarget.transform.position;
    }
    
}
