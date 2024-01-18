using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �A�^�b�`�����ۂɁA�����I��Inspecter��Component��ǉ�
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridLineDraw : MonoBehaviour
{
    // Inspector�p
    public float gridSize = 1f;       // �O���b�h(�l�p)�̃T�C�Y
    public Color color = Color.white; // �`����̐F
    public Vector2Int size = new Vector2Int(8, 8); // �c���Ɖ����̐�

    //[Header("�O���b�h�ʒu�����p")]
    //public Vector2 posRetouch = new Vector2(0.5f, 0.5f);

    // �l���ύX���ꂽ�Ƃ��p�ɁA����Inspector�̒l��ێ����Ă���
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
        UpdateGrid(); // �X�V����
    }

    private void UpdateGrid()
    {
        // �����y���A�l���X�V���ꂽ�Ƃ��Ƀ��b�V�����Ď擾
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
            // �A�C�e���T�C�Y�̕ύX
            if(mapManager.itemSize != gridSize)
            {
                mapManager.itemSize = gridSize;
            }
        }
    }

    /// <summary>
    /// mesh�̒l�v�Z���\�b�h
    /// �l���X�V����邽�тɌĂяo��
    /// </summary>
    /// <param name="mesh"></param>
    /// <returns></returns>
    private Mesh ReGrid(Mesh mesh)
    {
        // �O���b�h�`��Ŏg�p����}�e���A���̐ݒ�
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Sprites/Default"));

        mesh.Clear();

        // �����Əc���̒��_��
        int horizontalDrawSize = size.x * 2;
        int verticalDrawSize = size.y * 2;

        // �O���b�h�̉����Əc��
        float horizontalWidth = gridSize * horizontalDrawSize / 4.0f;
        float verticalWidth = gridSize * verticalDrawSize / 4.0f;

        // �����Əc���̎n�_�I�_
        Vector2 posRetouch = new Vector2(gridSize / 2, gridSize / 2);
        Vector2 horizontalStartPosition = new Vector2(-horizontalWidth + posRetouch.x, -verticalWidth + posRetouch.y);
        Vector2 verticalStartPosition = new Vector2(-horizontalWidth + posRetouch.x, -verticalWidth + posRetouch.y);
        Vector2 horizontalEndPosition = new Vector2(horizontalWidth + posRetouch.x, verticalWidth + posRetouch.y);
        Vector2 verticalEndPosition = new Vector2(horizontalWidth + posRetouch.x, verticalWidth + posRetouch.y);

        // �`��Ԋu
        float horizontalDiff = horizontalWidth / horizontalDrawSize;
        float verticalDiff = verticalWidth / verticalDrawSize;

        // �Ō�̂Q�ӂ�ǉ�(���_�̑���)
        int horizontalResolution = (horizontalDrawSize + 2) * 2;
        int verticalResolution = (verticalDrawSize + 2) * 2;

        // �����Əc���̒��_�AUV���W�A���C���̃C���f�b�N�X�A�F�̃f�[�^��������
        Vector3[] vertices = new Vector3[horizontalResolution + verticalResolution];
        Vector2[] uvs = new Vector2[horizontalResolution + verticalResolution];
        int[] lines = new int[horizontalResolution + verticalResolution];
        Color[] colors = new Color[horizontalResolution + verticalResolution];

        // �T�C�Y�ύX�p
        backGroundSize.x = Vector2.Distance(horizontalStartPosition, horizontalEndPosition);
        backGroundSize.y = Vector2.Distance(verticalStartPosition, verticalEndPosition);

        // �����̒��_��ݒ�
        for (int i = 0; i < horizontalResolution; i += 4)
        {
            vertices[i] = new Vector3(horizontalStartPosition.x + (horizontalDiff * (float)i), horizontalStartPosition.y, 0);
            vertices[i + 1] = new Vector3(horizontalStartPosition.x + (horizontalDiff * (float)i), horizontalEndPosition.y, 0);
            vertices[i + 2] = new Vector3(horizontalStartPosition.x, horizontalEndPosition.y - (horizontalDiff * (float)i), 0);
            vertices[i + 3] = new Vector3(horizontalEndPosition.x, horizontalEndPosition.y - (horizontalDiff * (float)i), 0);
        }

        // �c���̒��_��ݒ�
        for (int i = horizontalResolution; i < horizontalResolution + verticalResolution; i += 4)
        {
            vertices[i] = new Vector3(verticalStartPosition.x + (verticalDiff * (float)(i - horizontalResolution)), verticalStartPosition.y, 0);
            vertices[i + 1] = new Vector3(verticalStartPosition.x + (verticalDiff * (float)(i - horizontalResolution)), verticalEndPosition.y, 0);
            vertices[i + 2] = new Vector3(verticalStartPosition.x, verticalEndPosition.y - (verticalDiff * (float)(i - horizontalResolution)), 0);
            vertices[i + 3] = new Vector3(verticalEndPosition.x, verticalEndPosition.y - (verticalDiff * (float)(i - horizontalResolution)), 0);
        }

        // �S�Ă̒��_�ɑ΂���UV���W�A���C���̃C���f�b�N�X�A�F�̃f�[�^��ݒ�
        for (int i = 0; i < horizontalResolution + verticalResolution; i++)
        {
            uvs[i] = Vector2.zero;
            lines[i] = i;
            colors[i] = color;
        }

        Vector3 rotDirection = Vector3.forward;

        // ���_���w��̕����ɉ�]�ړ�������
        mesh.vertices = RotationVertices(vertices, rotDirection);
        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.SetIndices(lines, MeshTopology.Lines, 0);

        // �e�X�g
        // �p�l���̑傫���ݒ�
        //mapManager.GetGridSize(backGroundSize);

        // �X�V�O�̒l��ۑ�
        originGridSize = gridSize;
        originSize.x = size.x;
        originSize.y = size.y;
        originColor = color;

        return mesh;
    }

    /// <summary>
    /// ���_�z��f�[�^�[�����ׂĎw��̕����։�]�ړ������郁�\�b�h
    /// </summary>
    /// <param name="vertices">�����Əc���̒��_</param>
    /// <param name="rotDirection">�w��̕���</param>
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
