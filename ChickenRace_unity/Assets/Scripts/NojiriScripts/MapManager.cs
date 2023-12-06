using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public Vector2Int gridPos = new Vector2Int(2, -3); // �e�X�g�p�ݒu�ʒu

    [SerializeField] private GameObject gameObject; // �ړ��������I�u�W�F�N�g�̏��擾
    [SerializeField] private List<GameObject> InstalledList; // �ݒu������Q�����X�g
    [SerializeField] private List<Vector2Int> UsedGridList;   // �g�p�ς݃O���b�h�̈ʒu���X�g

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

        if (Input.GetMouseButtonDown(0))
        {
            GenerateMapObject();
        }
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
            ������
            �E�ړ����\�b�h
            �E�ݒu�ʒu�擾���\�b�h
            �E��Q���������\�b�h
            ���Ăяo��
            */

            yield return null;
        }

        Debug.Log("�R���[�`���I��");
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
    public void GenerateMapObject(/*id, Vector2Int gridPos*/)
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B�G���[�R");
            return;
        }

        //Vector3 installPos; // ���ݒu�ʒu

        // �N���b�N�ʒu�擾
        //installPos = Input.mousePosition;
        //installPos.x = Mathf.Round(installPos.x);
        //installPos.y = Mathf.Round(installPos.y);
        //installPos.z = 10.0f;

        //Debug.Log(installPos);

        // �J�[�\���̈ʒu�ɏ�Q���𐶐�
        // �����ʒu��z���������-10����邽�߁A10�ɐݒ肵�Ă���
        GameObject gameObj = (GameObject)Resources.Load("Square"); // ����Square
        Instantiate(gameObj, new Vector3(gridPos.x, gridPos.y, 10.0f)/*Camera.main.ScreenToWorldPoint(new Vector3Int(gridPos.x, gridPos.y, 10))*/, Quaternion.identity);
    }
    #endregion
}
