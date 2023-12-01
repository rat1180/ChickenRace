using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    Vector2Int ObstacleCenterPos; // 中心グリッド位置
    [SerializeField]
    List<Vector2Int> ObstacleCenterPosList; // 相対グリッド

    [SerializeField]
    int ObstacleID; // 障害物のID
    [SerializeField]
    int ObstacleKindID; // 種類のID
    [SerializeField]
    int MyRotation; // 回転
    [SerializeField]
    UnityEvent UE; // イベント関数
    void Awake()
    {
        Init();
    }
    /// <summary>
    /// 初期化
    /// </summary>
    void Init()
    {
        ObstacleCenterPos = new Vector2Int(0, 0);
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
        MyRotation = rot;
    }
    /// <summary>
    /// 破壊
    /// </summary>
    void Destoroy()
    {
        this.gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }
}
