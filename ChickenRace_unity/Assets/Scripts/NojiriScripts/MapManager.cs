using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Vector2Int gridPos = new Vector2Int(2, -3); // テスト用設置位置

    [SerializeField] private GameObject gameObject; // 移動したいオブジェクトの情報取得
    [SerializeField] private List<GameObject> InstalledList; // 設置した障害物リスト
    [SerializeField] private List<Vector2Int> UsedGridList;   // 使用済みグリッドの位置リスト

    private bool isRunning = false; // コルーチン実行判定フラグ
    private bool isInstall = false; // 設置フラグ

    // Start is called before the first frame update
    void Start()
    {
        MapInit(); // 初期化
    }

    // Update is called once per frame
    void Update()
    {
        // テスト用
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
    /// 設置判定メソッド
    /// 引数に渡されたグリッド位置が、設置可能かどうかを戻り値で返す
    /// ture：生成可能 / false：生成不可
    /// </summary>
    public bool JudgeInstall(Vector2Int installPos)
    {
        // 設置判定
        for (int i = 0; i < UsedGridList.Count; i++)
        {
            if (installPos == UsedGridList[i])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 障害物設置用メソッド
    /// JudgeInstallメソッドからtrueが返った時に呼ばれる
    /// ID、グリッド位置を取得後、その位置に障害物生成
    /// </summary>
    public void GenerateMapObject(/*id, Vector2Int gridPos*/)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。エラー３");
            return;
        }

        //Vector3 installPos; // 仮設置位置

        // クリック位置取得
        //installPos = Input.mousePosition;
        //installPos.x = Mathf.Round(installPos.x);
        //installPos.y = Mathf.Round(installPos.y);
        //installPos.z = 10.0f;

        //Debug.Log(installPos);

        // カーソルの位置に障害物を生成
        // 生成位置のz軸が勝手に-10されるため、10に設定しておく
        GameObject gameObj = (GameObject)Resources.Load("Square"); // 仮のSquare
        Instantiate(gameObj, new Vector3(gridPos.x, gridPos.y, 10.0f)/*Camera.main.ScreenToWorldPoint(new Vector3Int(gridPos.x, gridPos.y, 10))*/, Quaternion.identity);
    }
    #endregion
}
