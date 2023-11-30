using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Vector2Int ObstacleCenterPos;
    List<Vector2Int> ObstacleCenterPosList;

    int ObstacleID;
    int ObstacleKindID;

    //public enum KindOfObstacle
    //{
    //    Scaffold,
    //    MoveScaffold,
    //    Flower,
    //}
    //[SerializeField]
    //KindOfObstacle KOO;

    void Awake()
    {
        Init();
    }
    void Init()
    {
        ObstacleID = 1;
        ObstacleKindID = 1;
    }
    // Update is called once per frame
    void Update()
    {
        update();
    }
    public virtual void update()
    {

    }
    void Generation()
    {

    }
    void Destoroy()
    {
        this.gameObject.SetActive(false);
    }
}
