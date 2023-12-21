using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Surprise : MonoBehaviour
{
    [SerializeField]
    bool moveFlg;
    
    float speed;
    int counter;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    void Start()
    {
        moveFlg = false;
        speed = 0.05f;
        counter = 0;
    }
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            if (moveFlg && counter < 100)
            {
                this.transform.Translate(new Vector3(0, speed, 0));
                counter++;
                if (counter == 100)
                {
                    moveFlg = false;
                }
            }
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        moveFlg = true;
    }
}
