using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    [Header("�e�X�g�p�ݒu�I�u�W�F�N�g")]
    [SerializeField] private GameObject gameObj; // �ړ��������I�u�W�F�N�g�̏��擾

    [SerializeField] private List<int> InstalledList;       // �ݒu������Q�����X�g
    [SerializeField] private List<Vector2Int> UsedGridList; // �g�p�ς݃O���b�h�̈ʒu���X�g

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
        if (Input.GetKeyDown(KeyCode.X))
        {
            CreativeModeStart();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            CreativeModeEnd();
        }
    }

    #region ��������
    /// <summary>
    /// �}�b�v���������\�b�h
    /// </summary>
    private void MapInit()
    {
        // ���X�g�̏�����
        InstalledList = new List<int>();
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
    }
    #endregion

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
        isInstall = false;
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
    public void GenerateMapObject(int id, float angle, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B�G���[�R");
            return;
        }

        // ���������Q����I��
        //gameObj = (GameObject)Resources.Load("Square"); // ��Square

        // ��Q���̐���
        Instantiate(gameObj, new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));

        // �ݒu�����I�u�W�F�N�gID�ƈʒu�����X�g�ɒǉ�
        InstalledList.Add(id);
        UsedGridList.Add(gridPos);

        isInstall = true;
    }

    /// <summary>
    /// �t���O�Q�ƃ��\�b�h
    /// </summary>
    /// <returns>�ݒu�t���O�@true:�ݒu�ς݁@false:���ݒu</returns>
    public bool IsInstallReference()
    {
        return isInstall;
    }
    #endregion
}
