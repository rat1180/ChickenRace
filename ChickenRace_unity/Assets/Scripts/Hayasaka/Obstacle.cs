using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    protected Vector2Int obstacleCenterPos; // 中心グリッド位置
    [SerializeField]
    protected List<Vector2Int> collisionList; // 相対グリッド

    [SerializeField]
    protected int obstacleID; // 障害物のID
    [SerializeField]
    protected int obstacleKindID; // 種類のID
    [SerializeField]
    protected float myRotation; // 回転
    [SerializeField]
    protected UnityEvent ue; // イベント関数
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
        update();
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
    /// 破壊
    /// </summary>
    void Destoroy()
    {
        this.gameObject.SetActive(false);
        // Destroy(this.gameObject);
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
