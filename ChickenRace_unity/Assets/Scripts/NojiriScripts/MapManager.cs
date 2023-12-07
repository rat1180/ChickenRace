using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Vector3Int gridPos; // テスト用設置位置

    [SerializeField] private GameObject gameObject; // 移動したいオブジェクトの情報取得
    [SerializeField] private List<GameObject> InstalledList; // 設置した障害物リスト
    [SerializeField] private List<Vector2Int> UsedGridList;   // 使用済みグリッドの位置リスト
    [SerializeField] private Tilemap tilemap;

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

        //if (Input.GetMouseButtonDown(0))
        //{
        //    GenerateMapObject();
        //}
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
            コルーチン実行中に
            ・設置判定　　　：JudgeInstall()
            ・障害物生成　　：GenerateMapObject()
            ・コルーチン終了：CreativeModeEnd()
            のメソッドを呼び出せる
            */

            yield return null;
        }

        Debug.Log("コルーチン終了");
    }

    /// <summary>
    /// 設置位置確定メソッド
    /// 設置が確定した場合、設置位置を送る
    /// </summary>
    private Vector3 ConfirmPosition()
    {
        return Input.mousePosition;
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
                Debug.LogError("障害物が配置済みです");
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
    public void GenerateMapObject(int id, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。エラー３");
            return;
        }

        // カーソルの位置に障害物を生成
        GameObject gameObj = (GameObject)Resources.Load("Square"); // 仮のSquare

        // ワールド座標のマウス座標を取得
        // Mathf.RoundToIntでVector3からVector2Intに変換予定
        //Vector3 installPos = Camera.main.ScreenToWorldPoint(gridPos);
        Vector3 installPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //テスト

        // テスト
        //Vector3Int installPos;
        //gridPos = tilemap.WorldToCell(installPos);
        //Vector3 complementPos = new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
        //Vector3 worldPos = tilemap.CellToWorld(gridPos) + complementPos;
        //今選択している場所にオブジェクトを移動させる。
        //gameObj.transform.position = worldPos;

        // 座標を四捨五入、生成位置のz軸が勝手に-10されるため、0に設定しておく
        Vector2Int installPosInt;
        installPosInt = gridPos;
        //installPosInt = new Vector2Int(Mathf.RoundToInt(installPos.x), Mathf.RoundToInt(installPos.y));
        //installPos.y = Mathf.RoundToInt(installPos.y);
        //installPos.z = 0f;

        // 障害物の生成
        Instantiate(gameObj, new Vector3(installPosInt.x, installPosInt.y, 0f), Quaternion.identity);

        Debug.Log(installPos);

        // 設置したオブジェクトをリストに追加
        InstalledList.Add(gameObj);
        UsedGridList.Add(installPosInt);
    }
    #endregion
}
