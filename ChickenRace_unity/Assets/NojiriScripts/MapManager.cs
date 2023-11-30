using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public Vector2 movePos;    // 移動したい座標　テスト用

    [SerializeField] List<GameObject> GenerateList = new List<GameObject>(); // 生成した障害物リスト
    [SerializeField] List<Transform> UsedGridList = new List<Transform>(); // 設置した障害物リスト

    [SerializeField] private GameObject gameObject; // 移動したいオブジェクトの情報取得

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.tag == "UsedGrid")
    //    {
    //        Debug.Log("置けません");
    //    }
    //}

    /// <summary>
    /// グリッド移動メソッド
    /// </summary>
    private IEnumerator StartMove()
    {
        while (true)
        {
            // 移動メソッド
        }
    }

    #region 外部用メソッド
    /// <summary>
    /// コルーチン開始用メソッド
    /// 障害物の移動の際に呼び出す
    /// </summary>
    public void MapCoroutineStart()
    {
        StartCoroutine(StartMove());
    }

    /// <summary>
    /// コルーチン終了用メソッド
    /// </summary>
    public void MapCoroutineEnd()
    {
        StopCoroutine(StartMove());
    }

    /// <summary>
    /// 障害物設置時メソッド
    /// </summary>
    public void MapMoveEnd()
    {
        // その位置に固定
    }
    #endregion
}
