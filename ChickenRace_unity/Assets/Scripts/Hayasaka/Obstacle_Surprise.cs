using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Surprise : MonoBehaviour
{
    [SerializeField]
    bool isMoveFlg;
    [SerializeField]
    float speed;
    [SerializeField]
    int counter;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    void Start()
    {
        isMoveFlg = false;
        speed = 0.02f;
        counter = 0;
    }
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            if (isMoveFlg && counter < 100)
            {
                this.transform.Translate(new Vector3(0, speed, 0));
                counter++;
                if (counter == 100)
                {
                    isMoveFlg = false;
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Debug")
        {
            isMoveFlg = true;
        }
    }
}

