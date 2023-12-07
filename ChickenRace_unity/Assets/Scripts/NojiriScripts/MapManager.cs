using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Vector3Int gridPos; // �e�X�g�p�ݒu�ʒu

    [SerializeField] private GameObject gameObject; // �ړ��������I�u�W�F�N�g�̏��擾
    [SerializeField] private List<GameObject> InstalledList; // �ݒu������Q�����X�g
    [SerializeField] private List<Vector2Int> UsedGridList;   // �g�p�ς݃O���b�h�̈ʒu���X�g
    [SerializeField] private Tilemap tilemap;

    private bool isRunning = false; // �R���[�`�����s����t���O
    private bool isInstall = false; // �ݒu�t���O

    // Start is called before the first frame update
    void Start()
    {
        MapInit(); // ������
    }

    // Update is called once per frame
    void Update()
    {
        // �e�X�g�p
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
    /// �}�b�v���������\�b�h
    /// </summary>
    private void MapInit()
    {
        // ���X�g�̏�����
        InstalledList = new List<GameObject>();
        UsedGridList = new List<Vector2Int>();
    }

    /// <summary>
    /// ��Q���ݒu���[�h
    /// �O������R���[�`���̊J�n�A�I�����s��
    /// </summary>
    private IEnumerator CreativeMode()
    {
        // �R���[�`���I���܂Ń��[�v
        while (true)
        {
            /* 
            �R���[�`�����s����
            �E�ݒu����@�@�@�FJudgeInstall()
            �E��Q�������@�@�FGenerateMapObject()
            �E�R���[�`���I���FCreativeModeEnd()
            �̃��\�b�h���Ăяo����
            */

            yield return null;
        }

        Debug.Log("�R���[�`���I��");
    }

    /// <summary>
    /// �ݒu�ʒu�m�胁�\�b�h
    /// �ݒu���m�肵���ꍇ�A�ݒu�ʒu�𑗂�
    /// </summary>
    private Vector3 ConfirmPosition()
    {
        return Input.mousePosition;
    }

    #region �O���p���\�b�h
    /// <summary>
    /// �ݒu�J�n�p���\�b�h
    /// ��Q���ݒu���[�h�ڍs�̍ۂɃR���[�`���J�n
    /// </summary>
    public void CreativeModeStart()
    {
        if (isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���s���ł��B�G���[�P");
            return;
        }

        isRunning = true;
        StartCoroutine(CreativeMode());

        Debug.Log("�N���G�C�e�B�u���[�h�J�n");
    }

    /// <summary>
    /// �ݒu�I���p���\�b�h
    /// ��Q���ݒu�I����ɃR���[�`���I��
    /// </summary>
    public void CreativeModeEnd()
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B�G���[�Q");
            return;
        }

        isRunning = false;
        StopCoroutine(CreativeMode());

        Debug.Log("�N���G�C�e�B�u���[�h�I��");
    }

    /// <summary>
    /// �ݒu���胁�\�b�h
    /// �����ɓn���ꂽ�O���b�h�ʒu���A�ݒu�\���ǂ�����߂�l�ŕԂ�
    /// ture�F�����\ / false�F�����s��
    /// </summary>
    public bool JudgeInstall(Vector2Int installPos)
    {
        // �ݒu����
        for (int i = 0; i < UsedGridList.Count; i++)
        {
            if (installPos == UsedGridList[i])
            {
                Debug.LogError("��Q�����z�u�ς݂ł�");
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// ��Q���ݒu�p���\�b�h
    /// JudgeInstall���\�b�h����true���Ԃ������ɌĂ΂��
    /// ID�A�O���b�h�ʒu���擾��A���̈ʒu�ɏ�Q������
    /// </summary>
    public void GenerateMapObject(int id, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B�G���[�R");
            return;
        }

        // �J�[�\���̈ʒu�ɏ�Q���𐶐�
        GameObject gameObj = (GameObject)Resources.Load("Square"); // ����Square

        // ���[���h���W�̃}�E�X���W���擾
        // Mathf.RoundToInt��Vector3����Vector2Int�ɕϊ��\��
        //Vector3 installPos = Camera.main.ScreenToWorldPoint(gridPos);
        Vector3 installPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //�e�X�g

        // �e�X�g
        //Vector3Int installPos;
        //gridPos = tilemap.WorldToCell(installPos);
        //Vector3 complementPos = new Vector3(tilemap.cellSize.x / 2, tilemap.cellSize.y / 2, 0);
        //Vector3 worldPos = tilemap.CellToWorld(gridPos) + complementPos;
        //���I�����Ă���ꏊ�ɃI�u�W�F�N�g���ړ�������B
        //gameObj.transform.position = worldPos;

        // ���W���l�̌ܓ��A�����ʒu��z���������-10����邽�߁A0�ɐݒ肵�Ă���
        Vector2Int installPosInt;
        installPosInt = gridPos;
        //installPosInt = new Vector2Int(Mathf.RoundToInt(installPos.x), Mathf.RoundToInt(installPos.y));
        //installPos.y = Mathf.RoundToInt(installPos.y);
        //installPos.z = 0f;

        // ��Q���̐���
        Instantiate(gameObj, new Vector3(installPosInt.x, installPosInt.y, 0f), Quaternion.identity);

        Debug.Log(installPos);

        // �ݒu�����I�u�W�F�N�g�����X�g�ɒǉ�
        InstalledList.Add(gameObj);
        UsedGridList.Add(installPosInt);
    }
    #endregion
}
