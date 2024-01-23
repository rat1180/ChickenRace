using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ResorceNames;
using Dictionary;

/// <summary>
/// 障害物の情報保持用クラス
/// </summary>
[System.Serializable]
public class ObjectStatus
{
    [Header("オブジェクト設置方向")]
    public Dictionary_Unity<int, float> AngleList;        // <Key：設置順番, Value：設置向き情報>
    [Header("設置済みIDリスト")]
    public Dictionary_Unity<int, int> InstalledDic;       // <Key：設置順番, Value：障害物id情報>
    [Header("設置済み位置リスト")]
    public Dictionary_Unity<Vector2Int, int> usedGridDic; // <Key：設置済位置情報, Value：設置順番>
}

public class MapManager : MonoBehaviour
{
    [System.NonSerialized] public float itemSize;    // アイテムサイズ変更用
    public bool debugMode = false;                   // デバッグモードフラグ

    [SerializeField] private ObjectStatus objStatus; // 障害物用の構造体情報
    private GameObject obstacleObj;       // 移動したいオブジェクトの情報
    private GameObject gridObj;           // グリッド表示用オブジェクト
    private GameObject imageObj;          // 画像生成用オブジェクト取得
    private GameObject panelObj;          // グリッド表示用パネル
    private List<Vector2Int> childList;   // 障害物の子オブジェクトリスト
    private int installNum;               // 置かれた順番
    private bool isRunning = false;       // コルーチン実行判定フラグ
    private bool isInstall = true;        // 設置フラグ true：設置不可　false：設置可能

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        MapInit();
    }

    // Update is called once per frame
    void Update()
    {
        if (debugMode)
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
            if (Input.GetKeyDown(KeyCode.C))
            {
                ReInstallObject();
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
        objStatus.AngleList = new Dictionary_Unity<int, float>();
        objStatus.InstalledDic = new Dictionary_Unity<int, int>();
        objStatus.usedGridDic = new Dictionary_Unity<Vector2Int, int>();
        installNum = 0;

        // グリッドとパネルの情報を取得
        gridObj = Instantiate((GameObject)Resources.Load("GridObject"));
        imageObj = (GameObject)Resources.Load("ImageObject");
        panelObj = GameObject.Find("CanvasUI/GridPanel");

        itemSize = gridObj.GetComponent<GridLineDraw>().gridSize;

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
            ・障害物削除　　：DeleteObject()、RemoveObstacle()
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

    /// <summary>
    /// クリエイティブモード中のみ、障害物の静止画を表示
    /// </summary>
    private void GenerateImage()
    {
        // 設置済みIDの最大値を取得
        //var idMax = objStatus.InstalledDic.VelueMax();
        var idMax = 50;

        // 画像生成
        for (int i = 0; i <= idMax; i++)
        {
            // iと等しい設置済みid情報があるとき
            if (objStatus.InstalledDic.ContainValue(i))
            {
                var image = ResourceManager.instance.GetObstacleImage((OBSTACLE_IMAGE_NAMES)i); // idに対応した画像取得
                var installNumList = objStatus.InstalledDic.GetKeyList(i);                      // idに対応した設置順番をリスト取得

                for (int j = 0; j < installNumList.Count; j++)
                {
                    // 設置順番に対応した生成位置をリストで取得
                    var key = objStatus.usedGridDic.GetKey(installNumList[j]);

                    // イメージオブジェクトの生成
                    var obj = Instantiate(imageObj, new Vector3(key.x * itemSize, key.y * itemSize), Quaternion.identity);

                    //// サイズ変更
                    obj.transform.localScale = new Vector3(obj.transform.localScale.x * itemSize, obj.transform.localScale.y * itemSize);

                    // 静止画の適用
                    obj.GetComponent<SpriteRenderer>().sprite = image;

                    // MapManagerの子オブジェクトにする
                    obj.transform.parent = transform;
                }
            }
        }
    }

    /// <summary>
    /// 子オブジェクトの画像非表示メソッド
    /// </summary>
    private void DestroyImage()
    {
        Transform child = transform.gameObject.GetComponentInChildren<Transform>();

        if (child.childCount == 0) return;

        foreach (Transform obj in child)
        {
            Destroy(obj.gameObject);
        }
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
        //GenerateImage();

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
        isInstall = true;
        StopCoroutine(CreativeMode());
        GridDraw();
        //DestroyImage();

        Debug.Log("クリエイティブモード終了");
    }

    /// <summary>
    /// 設置判定メソッド
    /// 設置したいオブジェクトのグリッド位置が、設置可能かどうかを戻り値で返す
    /// ture：生成可能 / false：生成不可
    /// </summary>
    /// <param name="installPos">マウス位置(親の設置位置)</param>
    /// <param name="id">設置したいオブジェクト番号</param>
    /// <returns></returns>
    public bool JudgeInstall(Vector2Int installPos, int id)
    {
        // 消去オブジェクトの時の例外処理
        if ((OBSTACLE_OBJECT)id == OBSTACLE_OBJECT.Destroy_Bom)
        {
            return !JudgeInstallCenter(installPos);
        }
        else
        {
            // CollisionListを取得
            var obj = ResourceManager.instance.GetObstacleObject((OBSTACLE_OBJECT)id);
            childList = obj.GetComponent<Obstacle>().GetCollisionList();
        }

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

    /// <summary>
    /// 設置する位置判定メソッド
    /// 設置するオブジェクトが１マスの場合
    /// ture：生成可能 / false：生成不可
    /// </summary>
    /// <param name="pos">マウス位置(親の設置位置)</param>
    /// <returns></returns>
    private bool JudgeInstallCenter(Vector2Int pos)
    {
        // 設置済リスト内に、posと同じ位置情報があるとき
        if (objStatus.usedGridDic.ContainKey(pos)) return false;

        return true;
    }

    /// <summary>
    /// 設置する位置判定メソッド
    /// 設置するオブジェクトが２マス以上ある場合
    /// ture：生成可能 / false：生成不可
    /// </summary>
    /// <param name="pos">マウス位置(親の設置位置)</param>
    /// <param name="list">子の設置位置リスト</param>
    /// <returns></returns>
    private bool JudgeInstallChild(Vector2Int pos, List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var childInsPos = pos + list[i];

            // 設置済リスト内に、posと同じ位置情報があるとき
            if (objStatus.usedGridDic.ContainKey(childInsPos)) return false;
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

        // 消去オブジェクトの時の例外処理
        if ((OBSTACLE_OBJECT)id == OBSTACLE_OBJECT.Destroy_Bom)
        {
            DeleteObject(gridPos);
            isInstall = true;
            return;
        }

        // 障害物生成
        SpawnObstacle(id, angle, gridPos);

        if (!debugMode) // オフの時
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
        //if (!isRunning)
        //{
        //    Debug.LogError("クリエイティブモードが開始されていません。");
        //    return;
        //}

        // 障害物の取得
        obstacleObj = GetObstaclePrefab(id);

        // 生成する障害物のサイズ変更
        obstacleObj.transform.localScale = new Vector3(itemSize, itemSize,1);

        // 障害物の生成
        Instantiate(obstacleObj, new Vector3(gridPos.x * itemSize, gridPos.y * itemSize), Quaternion.Euler(0, 0, angle));

        // 設置したオブジェクトIDを追加
        objStatus.InstalledDic.Add(installNum, id);

        // 設置した位置情報と設置順番を追加
        objStatus.usedGridDic.Add(gridPos, installNum);

        if (childList != null) // 子オブジェクトがあるとき
        {
            for (int i = 0; i < childList.Count; i++)
            {
                // 親オブジェクトの座標を子オブジェクトの個数分追加
                objStatus.usedGridDic.Add(gridPos + childList[i], installNum);
            }
        }
        objStatus.AngleList.Add(installNum, angle);

        installNum++;
    }

    /// <summary>
    /// 削除処理実行用メソッド
    /// RemoveObstacleメソッドにて障害物削除後、全プレイヤーが同じメソッドを実行
    /// </summary>
    /// <param name="id"></param>
    /// <param name="gridPos"></param>
    public void DeleteObject(Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。");
            return;
        }

        // 障害物削除
        RemoveObstacle(gridPos);

        if (!debugMode) // オフの時
        {
            // 他のプレイヤーでRemoveObstacleメソッドの実行
            var Obj = PhotonNetwork.Instantiate("GenerateObstacle", new Vector3(0, 0), Quaternion.identity);
            Obj.GetComponent<GenerateObstacle>().DeleteObstacle(gridPos);
        }
    }

    /// <summary>
    /// 障害物爆破用メソッド
    /// 指定されたマスに障害物があるとき、その障害物を削除
    /// 複数マスを持っている障害物の場合、指定されたマス以外の同じキーを持った障害物を削除
    /// </summary>
    /// <param name="deletePos">Vector2Int型のマウス位置（TKey）</param>
    public void RemoveObstacle(Vector2Int deletePos)
    {
        if (!isRunning)
        {
            Debug.LogError("クリエイティブモードが開始されていません。");
            return;
        }

        // 削除したい位置情報から、Value(id)を取得
        var deleteNum = objStatus.usedGridDic.GetValue(deletePos);

        // 要素の削除
        if (childList == null)
        {
            objStatus.usedGridDic.Remove(deletePos);
        }
        else
        {
            // 取得したValue(id)に対応したKeyリストを取得
            var keyList = objStatus.usedGridDic.GetKeyList(deleteNum);

            // Keyリストの要素を削除
            foreach (var list in keyList)
            {
                objStatus.usedGridDic.Remove(list);
            }
        }

        // Value(id)に対応した、それぞれのリスト要素を削除
        objStatus.InstalledDic.Remove(deleteNum);
        objStatus.AngleList.Remove(deleteNum);
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

        // 再生成
        for (int i = 0; i < installNum; i++)
        {
            // Keyに対応したValue(id)のオブジェクト取得
            obstacleObj = GetObstaclePrefab(objStatus.InstalledDic.GetValue(i));

            // 生成する障害物のサイズ変更
            obstacleObj.transform.localScale = new Vector3(itemSize, itemSize);

            // iと等しいValueがあるとき
            if (objStatus.usedGridDic.ContainValue(i))
            {
                var key = objStatus.usedGridDic.GetKey(i);   // 生成位置をリストで取得
                var angle = objStatus.AngleList.GetValue(i); // 生成角度を取得

                // 親オブジェクトの生成
                Instantiate(obstacleObj, new Vector3(key.x * itemSize, key.y * itemSize), Quaternion.Euler(0, 0, angle));
            }
        }
    }

    /// <summary>
    /// 設置状態フラグ参照メソッド
    /// </summary>
    /// <returns>設置フラグ　true:設置不可　false:設置可能</returns>
    public bool IsInstallReference()
    {
        return isInstall;
    }
    #endregion
}
