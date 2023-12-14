using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MapManager : MonoBehaviour
{
    [SerializeField] private GameObject gameObj; // 移動したいオブジェクトの情報取得
    [SerializeField] private List<int> InstalledList;       // 設置した障害物リスト
    [SerializeField] private List<Vector2Int> UsedGridList; // 使用済みグリッドの位置リスト

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
        if (Input.GetKeyDown(KeyCode.X))
        {
            CreativeModeStart();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CreativeModeEnd();
        }
    }

    #region 内部処理
    /// <summary>
    /// マップ初期化メソッド
    /// </summary>
    private void MapInit()
    {
        // リストの初期化
        InstalledList = new List<int>();
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
    }

    /// <summary>
    /// ResourcesManagerからidに対応するオブジェクトを取得する
    /// </summary>
    private GameObject GetObstaclePrefab()
    {
        return gameObj;
    }
    #endregion

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
        isInstall = false;
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
    /// <param name="id">生成するオブジェクト番号</param>
    /// <param name="angle">生成する際の向き</param>
    /// <param name="gridPos">生成する位置</param>
    public void GenerateMapObject(int id, float angle, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。エラー３");
            return;
        }

        // 障害物生成
        SpawnObstacle(id, angle, gridPos);

        // 他のプレイヤーでSpawnObstacleメソッドの実行
        var Obj = PhotonNetwork.Instantiate("GenerateObstacle", new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));
        Obj.GetComponent<GenerateObstacle>().SetObstacleID(id, angle, gridPos);

        isInstall = true;
    }

    /// <summary>
    /// 障害物の生成メソッド
    /// 同時に他プレイヤーから呼ばれる
    /// </summary>
    /// <param name="id">生成するオブジェクト番号</param>
    /// <param name="angle">生成する際の向き</param>
    /// <param name="gridPos">生成する位置</param>
    public void SpawnObstacle(int id, float angle, Vector2Int gridPos)
    {
        // 障害物の取得
        gameObj = GetObstaclePrefab();

        // 障害物の生成
        Instantiate(gameObj, new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));

        // 設置したオブジェクトIDと位置をリストに追加
        InstalledList.Add(id);
        UsedGridList.Add(gridPos);
    }

    /// <summary>
    /// フラグ参照メソッド
    /// </summary>
    /// <returns>設置フラグ　true:設置済み　false:未設置</returns>
    public bool IsInstallReference()
    {
        return isInstall;
    }
    #endregion
}
