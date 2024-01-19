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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player") collision.transform.parent = transform;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.transform.parent == transform)
            {
                collision.transform.parent = null;
            }
        }
    }

}
