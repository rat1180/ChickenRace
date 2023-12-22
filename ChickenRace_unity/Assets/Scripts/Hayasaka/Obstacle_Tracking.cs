using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Tracking : MonoBehaviour
{
    [SerializeField]
    GameObject trackingTarget;
   
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            Tracking();
        }
    }
    /// <summary>
    /// –_‚É‚­‚Á‚Â‚­
    /// </summary>
    void Tracking()
    {
        this.transform.position = trackingTarget.transform.position;
    }
    
}
