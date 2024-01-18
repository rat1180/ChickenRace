using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アタッチした際に、自動的にInspecterにComponentを追加
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridLineDraw : MonoBehaviour
{
    // Inspector用
    public float gridSize = 1f;       // グリッド(四角)のサイズ
    public Color color = Color.white; // 描画線の色
    public Vector2Int size = new Vector2Int(8, 8); // 縦線と横線の数

    //[Header("グリッド位置調整用")]
    //public Vector2 posRetouch = new Vector2(0.5f, 0.5f);

    // 値が変更されたとき用に、元のInspectorの値を保持しておく
    private float originGridSize = 0;
    private Color originColor = Color.white;
    private Vector2Int originSize = new Vector2Int(0, 0);
    private Vector2 backGroundSize;

    private GameObject mapObj;
    private MapManager mapManager;
    private Mesh mesh;

    void Start()
    {
         mapObj = GameManager.instance.GetMapManager();
        if(mapObj ==null) mapObj = GameObject.Find("MapManager");
        mapManager = mapObj.GetComponent<MapManager>();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh = ReGrid(mesh);
    }

    void Update()
    {
        UpdateGrid(); // 更新処理
    }

    private void UpdateGrid()
    {
        // 処理軽減、値が更新されたときにメッシュを再取得
        if (gridSize != originGridSize || size.x != originSize.x ||
            size.y != originSize.y || originColor != color)
        {
            if (gridSize < 0) { gridSize = 0; }
            if (size.x < 0) { size.x = 1; }
            if (size.y < 0) { size.y = 1; }
            ReGrid(mesh);
        }

        if(!mapManager.IsInstallReference())
        {
            // アイテムサイズの変更
            if(mapManager.itemSize != gridSize)
            {
                mapManager.itemSize = gridSize;
            }
        }
    }

    /// <summary>
    /// meshの値計算メソッド
    /// 値が更新されるたびに呼び出す
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private Mesh ReGrid(Mesh mesh)
    {
        // グリッド描画で使用するマテリアルの設定
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));

        mesh.Clear();

        // 横線と縦線の頂点数
        int horizontalDrawSize = size.x * 2;
        int verticalDrawSize = size.y * 2;

        // グリッドの横幅と縦幅
        float horizontalWidth = gridSize * horizontalDrawSize / 4.0f;
        float verticalWidth = gridSize * verticalDrawSize / 4.0f;

        // 横線と縦線の始点終点
        Vector2 posRetouch = new Vector2(gridSize / 2, gridSize / 2);
        Vector2 horizontalStartPosition = new Vector2(-horizontalWidth + posRetouch.x, -verticalWidth + posRetouch.y);
        Vector2 verticalStartPosition = new Vector2(-horizontalWidth + posRetouch.x, -verticalWidth + posRetouch.y);
        Vector2 horizontalEndPosition = new Vector2(horizontalWidth + posRetouch.x, verticalWidth + posRetouch.y);
        Vector2 verticalEndPosition = new Vector2(horizontalWidth + posRetouch.x, verticalWidth + posRetouch.y);

        // 描画間隔
        float horizontalDiff = horizontalWidth / horizontalDrawSize;
        float verticalDiff = verticalWidth / verticalDrawSize;

        // 最後の２辺を追加(頂点の総数)
        int horizontalResolution = (horizontalDrawSize + 2) * 2;
        int verticalResolution = (verticalDrawSize + 2) * 2;

        // 横線と縦線の頂点、UV座標、ラインのインデックス、色のデータを初期化
        Vector3[] vertices = new Vector3[horizontalResolution + verticalResolution];
        Vector2[] uvs = new Vector2[horizontalResolution + verticalResolution];
        int[] lines = new int[horizontalResolution + verticalResolution];
        Color[] colors = new Color[horizontalResolution + verticalResolution];

        // サイズ変更用
        backGroundSize.x = Vector2.Distance(horizontalStartPosition, horizontalEndPosition);
        backGroundSize.y = Vector2.Distance(verticalStartPosition, verticalEndPosition);

        // 横線の頂点を設定
        for (int i = 0; i < horizontalResolution; i += 4)
        {
            vertices[i] = new Vector3(horizontalStartPosition.x + (horizontalDiff * (float)i), horizontalStartPosition.y, 0);
            vertices[i + 1] = new Vector3(horizontalStartPosition.x + (horizontalDiff * (float)i), horizontalEndPosition.y, 0);
            vertices[i + 2] = new Vector3(horizontalStartPosition.x, horizontalEndPosition.y - (horizontalDiff * (float)i), 0);
            vertices[i + 3] = new Vector3(horizontalEndPosition.x, horizontalEndPosition.y - (horizontalDiff * (float)i), 0);
        }

        // 縦線の頂点を設定
        for (int i = horizontalResolution; i < horizontalResolution + verticalResolution; i += 4)
        {
            vertices[i] = new Vector3(verticalStartPosition.x + (verticalDiff * (float)(i - horizontalResolution)), verticalStartPosition.y, 0);
            vertices[i + 1] = new Vector3(verticalStartPosition.x + (verticalDiff * (float)(i - horizontalResolution)), verticalEndPosition.y, 0);
            vertices[i + 2] = new Vector3(verticalStartPosition.x, verticalEndPosition.y - (verticalDiff * (float)(i - horizontalResolution)), 0);
            vertices[i + 3] = new Vector3(verticalEndPosition.x, verticalEndPosition.y - (verticalDiff * (float)(i - horizontalResolution)), 0);
        }

        // 全ての頂点に対してUV座標、ラインのインデックス、色のデータを設定
        for (int i = 0; i < horizontalResolution + verticalResolution; i++)
        {
            uvs[i] = Vector2.zero;
            lines[i] = i;
            colors[i] = color;
        }

        Vector3 rotDirection = Vector3.forward;

        // 頂点を指定の方向に回転移動させる
        mesh.vertices = RotationVertices(vertices, rotDirection);
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        // テスト
        // パネルの大きさ設定
        //mapManager.GetGridSize(backGroundSize);

        // 更新前の値を保存
        originGridSize = gridSize;
        originSize.x = size.x;
        originSize.y = size.y;
        originColor = color;

        return mesh;
    }

    /// <summary>
    /// 頂点配列データーをすべて指定の方向へ回転移動させるメソッド
    /// </summary>
    /// <param name="vertices">横線と縦線の頂点</param>
    /// <param name="rotDirection">指定の方向</param>
    /// <returns></returns>
    private Vector3[] RotationVertices(Vector3[] vertices, Vector3 rotDirection)
    {
        Vector3[] ret = new Vector3[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            ret[i] = Quaternion.LookRotation(rotDirection) * vertices[i];
        }
        return ret;
    }
}
