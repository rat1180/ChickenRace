using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ResorceNames;

/// <summary>
/// 障害物の情報保持用クラス
/// </summary>
[System.Serializable]
public struct ObjectStatus
{
    public List<int> InstalledList;        // 設置した障害物idリスト
    public List<float> AngleList;          // 障害物の設置方向リスト
    public List<Vector2Int> UsedGridList;  // 使用済みグリッドの位置リスト
    public List<Vector2Int> testList; // テスト用
}

public class MapManager : MonoBehaviour
{
    public bool debugMode = false; // デバッグモードフラグ

    [SerializeField] private ObjectStatus objStatus; // 障害物用の構造体情報
    private GameObject obstacleObj; // 移動したいオブジェクトの情報取得
    private GameObject gridObj;
    private GameObject panelObj;
    private List<Vector2Int> childList;
    private Vector2 panelSize;

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
        if (debugMode)
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                CreativeModeStart();
            }
            if (Input.GetKeyDown(KeyCode.Z))
            {
                CreativeModeEnd();
            }
        }
    }

    #region 内部処理
    /// <summary>
    /// マップ初期化メソッド
    /// </summary>
    private void MapInit()
    {
        // 初期化
        objStatus.InstalledList = new List<int>();
        objStatus.AngleList = new List<float>();
        objStatus.UsedGridList = new List<Vector2Int>();
        objStatus.testList = new List<Vector2Int>();     // テスト
        obstacleObj = new GameObject();

        // テスト用
        if (debugMode)
        {
            objStatus.testList.Add(new Vector2Int(0, 1));
            objStatus.testList.Add(new Vector2Int(-1, 0));
        }

        // グリッドとパネルの情報を取得
        gridObj = Instantiate((GameObject)Resources.Load("GridObject"));
        panelObj = GameObject.Find("CanvasUI/GridPanel");
        panelSize = panelObj.transform.GetComponent<RectTransform>().sizeDelta;

        // 初期は非表示
        gridObj.SetActive(false);
        panelObj.SetActive(false);
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
            コルーチン実行時
            ・設置判定　　　：JudgeInstall()
            ・障害物生成　　：GenerateMapObject()、SpawnObstacle()
            ・コルーチン終了：CreativeModeEnd()

            コルーチン未実行時
            ・障害物再配置　：ReInstallObject()
            ・コルーチン開始：CreativeModeStart()

            上記のメソッドを呼び出せる
            */

            yield return null;
        }
    }

    /// <summary>
    /// ResourcesManagerからidに対応するオブジェクトを取得する
    /// </summary>
    private GameObject GetObstaclePrefab(int id)
    {
        var obj = ResourceManager.instance.GetObstacleObject((OBSTACLE_OBJECT)id);

        return obj;
    }

    /// <summary>
    /// グリッド描画切替メソッド
    /// クリエイティブモードの状態によって切り替え
    /// </summary>
    private void GridDraw()
    {
        if (!isRunning)
        {
            Debug.Log("グリッド表示ができない状態です。");
            gridObj.SetActive(false);
            panelObj.SetActive(false);
            return;
        }

        gridObj.SetActive(true);
        panelObj.SetActive(true);
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
            Debug.LogError("クリエイティブモード実行中です。");
            return;
        }

        isRunning = true;
        isInstall = false;
        StartCoroutine(CreativeMode());
        GridDraw();

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
            Debug.LogError("クリエイティブモードが開始されていません。");
            return;
        }

        isRunning = false;
        StopCoroutine(CreativeMode());
        GridDraw();

        Debug.Log("クリエイティブモード終了");
    }

    /// <summary>
    /// 設置判定メソッド
    /// 設置したいオブジェクトのグリッド位置が、設置可能かどうかを戻り値で返す
    /// ture：生成可能 / false：生成不可
    /// </summary>
    /// <param name="installPos">マウスのグリッド位置</param>
    /// <param name="id">設置したいオブジェクト番号</param>
    /// <returns></returns>
    public bool JudgeInstall(Vector2Int installPos, int id)
    {
        // idに対応したリストを取得
        childList = objStatus.testList;
        //var Obj = ResourceManager.instance.GetObstacleObject((OBSTACLE_OBJECT)id);
        //childList = Obj.GetComponent<Obstacle>().Seter();

        // 設置位置が一つのとき
        if (childList == null)
        {
            return JudgeInstallCenter(installPos);
        }
        else // 一つでない時
        {
            // 全てのマスで設置可能かどうか
            if (JudgeInstallCenter(installPos) && JudgeInstallChild(installPos, childList))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    // 消す予定
    public bool JudgeInstall(Vector2Int installPos)
    {
        return true;
    }

    /// <summary>
    /// 設置する位置判定メソッド
    /// 設置するオブジェクトが１マスの場合
    /// </summary>
    /// <param name="pos">マウスのグリッド位置</param>
    /// <returns></returns>
    private bool JudgeInstallCenter(Vector2Int pos)
    {
        for (int i = 0; i < objStatus.UsedGridList.Count; i++)
        {
            if( pos == objStatus.UsedGridList[i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 設置する位置判定メソッド
    /// 設置するオブジェクトが２マス以上ある場合
    /// </summary>
    /// <param name="pos">マウスのグリッド位置</param>
    /// <param name="list">pos以外の位置リスト</param>
    /// <returns></returns>
    private bool JudgeInstallChild(Vector2Int pos, List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var childInsPos = pos + list[i];

            for (int j = 0; j < objStatus.UsedGridList.Count; j++)
            {
                if (childInsPos == objStatus.UsedGridList[j])
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// 全プレイヤー障害物生成メソッド
    /// ID、生成方向、グリッド位置を取得
    /// SpawnObstacleメソッドにて障害物生成後、全プレイヤーが同じメソッドを実行
    /// </summary>
    /// <param name="id">生成するオブジェクト番号</param>
    /// <param name="angle">生成する際の向き</param>
    /// <param name="gridPos">生成する位置</param>
    public void GenerateMapObject(int id, float angle, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。");
            return;
        }

        // 障害物生成
        SpawnObstacle(id, angle, gridPos);

        if (!debugMode)
        {
            // 他のプレイヤーでSpawnObstacleメソッドの実行
            var Obj = PhotonNetwork.Instantiate("GenerateObstacle", new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));
            Obj.GetComponent<GenerateObstacle>().SetObstacleID(id, angle, gridPos);
        }

        isInstall = true;
    }

    /// <summary>
    /// 障害物生成メソッド
    /// </summary>
    /// <param name="id">生成するオブジェクト番号</param>
    /// <param name="angle">生成する際の向き</param>
    /// <param name="gridPos">生成する位置</param>
    public void SpawnObstacle(int id, float angle, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。");
            return;
        }

        // 障害物の取得
        obstacleObj = GetObstaclePrefab(id);

        // 障害物の生成
        Instantiate(obstacleObj, new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));

        // 設置したオブジェクトIDと位置をリストに追加
        objStatus.InstalledList.Add(id);
        objStatus.UsedGridList.Add(gridPos);
        if(childList != null)
        {
            for(int i = 0; i < childList.Count; i++)
            {
                objStatus.UsedGridList.Add(gridPos + childList[i]);
            }
        }
        objStatus.AngleList.Add(angle);
    }

    /// <summary>
    /// 障害物の再設置メソッド
    /// ラウンド終了時、障害物を再設置することで初期状態に戻す
    /// </summary>
    public void ReInstallObject()
    {
        if (isRunning)
        {
            Debug.Log("クリエイティブモードが実行中のため、再配置できません。");
            return;
        }

        for (int i = 0; i < objStatus.InstalledList.Count; i++)
        {
            obstacleObj = GetObstaclePrefab(objStatus.InstalledList[i]);
            Instantiate(obstacleObj, new Vector3(objStatus.UsedGridList[i].x, objStatus.UsedGridList[i].y), Quaternion.Euler(0, 0, objStatus.AngleList[i]));
        }
    }

    /// <summary>
    /// フラグ参照メソッド
    /// </summary>
    /// <returns>設置フラグ　true:設置済み　false:未設置</returns>
    public bool IsInstallReference()
    {
        return isInstall;
    }

    // テスト
    //public void GetGridSize(Vector2 gridSize)
    //{
    //    // サイズ変わらん
    //    panelSize = gridSize;
    //}
    #endregion
}
