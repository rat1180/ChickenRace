using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// アタッチした際に、自動的にInspecterにComponentを追加
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridLineDraw : MonoBehaviour
{
    // Inspector用
    public float gridSize = 1f;
    public Vector2Int size = new Vector2Int(8, 8); // 縦線と横線の数
    public Color color = Color.white;
    public bool back = true;

    // 値が変更されたとき用に、元のInspectorの値を保持しておく
    float originGridSize = 0;
    Vector2Int originSize = new Vector2Int();
    Color originColor = Color.white;
    bool originBack = true;

    Mesh mesh;

    void Start()
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh = ReGrid(mesh);
    }

    void Update()
    {
        // 処理軽減、値が更新されたときにメッシュを再取得
        if (gridSize != originGridSize || size.x != originSize.x ||
            size.y != originSize.y || originColor != color || originBack != back)
        {
            if (gridSize < 0) { gridSize = 0.000001f; }
            if (size.x < 0) { size.x = 1; }
            if (size.y < 0) { size.y = 1; }
            ReGrid(mesh);
        }
    }

    private Mesh ReGrid(Mesh mesh)
    {
        if (back)
        {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));
        }
        else
        {
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("GUI/Text Shader"));
        }

        mesh.Clear();

        // 横線と縦線の頂点数
        int horizontalDrawSize = size.x * 2;
        int verticalDrawSize = size.y * 2;

        // グリッドの横幅と縦幅
        float horizontalWidth = gridSize * horizontalDrawSize / 4.0f;
        float verticalWidth = gridSize * verticalDrawSize / 4.0f;

        // 横線と縦線の始点終点
        Vector2 horizontalStartPosition = new Vector2(-horizontalWidth, -verticalWidth);
        Vector2 verticalStartPosition = new Vector2(-horizontalWidth, -verticalWidth);
        Vector2 horizontalEndPosition = new Vector2(horizontalWidth, verticalWidth);
        Vector2 verticalEndPosition = new Vector2(horizontalWidth, verticalWidth);

        // 描画間隔
        float horizontalDiff = horizontalWidth / horizontalDrawSize;
        float verticalDiff = verticalWidth / verticalDrawSize;

        // 最後の２辺を追加(頂点の総数)
        int horizontalResolution = (horizontalDrawSize + 2) * 2;
        int verticalResolution = (verticalDrawSize + 2) * 2;

        Vector3[] vertices = new Vector3[horizontalResolution + verticalResolution];
        Vector2[] uvs = new Vector2[horizontalResolution + verticalResolution];
        int[] lines = new int[horizontalResolution + verticalResolution];
        Color[] colors = new Color[horizontalResolution + verticalResolution];

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

        for (int i = 0; i < horizontalResolution + verticalResolution; i++)
        {
            uvs[i] = Vector2.zero;
            lines[i] = i;
            colors[i] = color;
        }

        Vector3 rotDirection = Vector3.forward;

        mesh.vertices = RotationVertices(vertices, rotDirection);
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        originGridSize = gridSize;
        originSize.x = size.x;
        originSize.y = size.y;
        originColor = color;
        originBack = back;

        return mesh;
    }

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
