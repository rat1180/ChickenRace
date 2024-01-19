using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int obstacleCenterPos;   // 中心グリッド位置
    [SerializeField]
    protected List<Vector2Int> collisionList; // 相対グリッド

    [SerializeField]
    protected int obstacleID; // 障害物のID
    [SerializeField]
    protected int obstacleKindID; // 種類のID
    [SerializeField]
    protected float myRotation; // 生成時の角度を受け取る
    [SerializeField]
    protected UnityEvent ue;    // イベント関数
    void Awake()
    {
        Init();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    protected virtual void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            update();
        }
        if (GameManager.instance.CheckRaceEnd())
        {
            Destoroy();
        }
    }
    protected virtual void update()
    {
        
    }
    /// <summary>
    /// 生成時の回転等の情報受け取り
    /// </summary>
    /// <param name="rot"></param>
    protected void Generation(int rot)
    {
        myRotation = rot;
    }
    /// <summary>
    /// 破壊される
    /// </summary>
    void Destoroy()
    {
        //this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// リスト取得
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetCollisionList()
    {
        return collisionList;
    }
    protected virtual void ObjStart()
    {

    }
}
