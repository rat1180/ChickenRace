using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public Vector2 movePos;    // 移動したい座標　テスト用

    [SerializeField] private GameObject gameObject; // 移動したいオブジェクトの情報取得
    [SerializeField] private List<GameObject> InstalledList; // 設置した障害物リスト
    [SerializeField] private List<Vector2Int> UsedGridList;   // 使用済みグリッドの位置リスト

    private bool isRunning = false;  // コルーチン実行判定フラグ
    private bool isInstall = false; // 設置フラグ

    // Start is called before the first frame update
    void Start()
    {
        MapInit(); // 初期化
    }

    // Update is called once per frame
    void Update()
    {
        // テスト
        if (Input.GetKeyDown(KeyCode.S))
        {
            CreativeModeStart();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            CreativeModeEnd();
        }

        if (Input.GetMouseButtonDown(0))
        {
            GenerateMapObject();
        }
    }

    /// <summary>
    /// マップ初期化メソッド
    /// </summary>
    private void MapInit()
    {
        // リストの初期化
        InstalledList = new List<GameObject>();
        UsedGridList = new List<Vector2Int>();
    }

    /// <summary>
    /// 障害物設置モード
    /// 外部からコルーチンの開始、終了を行う
    /// </summary>
    private IEnumerator CreativeMode()
    {
        // コルーチン終了までループ
        while (true)
        {
            /* 
            ここで
            ・移動メソッド
            ・設置位置取得メソッド
            ・障害物生成メソッド
            を呼び出す
            */

            yield return null;
        }

        Debug.Log("コルーチン終了");
    }

    #region 外部用メソッド
    /// <summary>
    /// 設置開始用メソッド
    /// 障害物設置モード移行の際にコルーチン開始
    /// </summary>
    public void CreativeModeStart()
    {
        if (isRunning)
        {
            Debug.LogError("クリエイティブモード実行中です。エラー１");
            return;
        }

        isRunning = true;
        StartCoroutine(CreativeMode());

        Debug.Log("クリエイティブモード開始");
    }

    /// <summary>
    /// 設置終了用メソッド
    /// 障害物設置終了後にコルーチン終了
    /// </summary>
    public void CreativeModeEnd()
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。エラー２");
            return;
        }

        isRunning = false;
        StopCoroutine(CreativeMode());

        Debug.Log("クリエイティブモード終了");
    }

    /// <summary>
    /// 障害物設置用メソッド
    /// クリックされた位置を取得後、設置できるかを判定
    /// ture：障害物を生成 / false：生成しない
    /// </summary>
    public void GenerateMapObject()
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。エラー３");
            return;
        }

        Vector2Int installPos; // 仮設置位置

        // カーソルの位置に障害物を生成
        

        // クリック位置取得
        //Debug.Log(installPos);

        // 設置判定

        // その位置に固定
    }
    #endregion
}
