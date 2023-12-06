using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    protected Vector2Int obstacleCenterPos; // 中心グリッド位置
    [SerializeField]
    List<Vector2Int> collisionList; // 相対グリッド

    [SerializeField]
    int obstacleID; // 障害物のID
    [SerializeField]
    int obstacleKindID; // 種類のID
    [SerializeField]
    int myRotation; // 回転
    [SerializeField]
    UnityEvent ue; // イベント関数
    void Awake()
    {
        Init();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    public virtual void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        update();
    }
    public virtual void update()
    {
        
    }
    /// <summary>
    /// 生成時の回転等の情報受け取り
    /// </summary>
    /// <param name="rot"></param>
    public void Generation(int rot)
    {
        myRotation = rot;
    }
    /// <summary>
    /// 破壊
    /// </summary>
    void Destoroy()
    {
        this.gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }
    List<Vector2Int> Seter()
    {
        return collisionList;
    }
    public virtual void ObjStart()
    {

    }
}
