using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public Vector2 movePos;    // �ړ����������W�@�e�X�g�p

    [SerializeField] private GameObject gameObject; // �ړ��������I�u�W�F�N�g�̏��擾
    [SerializeField] private List<GameObject> InstalledList; // �ݒu������Q�����X�g
    [SerializeField] private List<Vector2Int> UsedGridList;   // �g�p�ς݃O���b�h�̈ʒu���X�g

    private bool isRunning = false;  // �R���[�`�����s����t���O
    private bool isInstall = false; // �ݒu�t���O

    // Start is called before the first frame update
    void Start()
    {
        MapInit(); // ������
    }

    // Update is called once per frame
    void Update()
    {
        // �e�X�g
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
    /// ��Q���ݒu�p���\�b�h
    /// �N���b�N���ꂽ�ʒu���擾��A�ݒu�ł��邩�𔻒�
    /// ture�F��Q���𐶐� / false�F�������Ȃ�
    /// </summary>
    public void GenerateMapObject()
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B�G���[�R");
            return;
        }

        Vector2Int installPos; // ���ݒu�ʒu

        // �J�[�\���̈ʒu�ɏ�Q���𐶐�
        

        // �N���b�N�ʒu�擾
        //Debug.Log(installPos);

        // �ݒu����

        // ���̈ʒu�ɌŒ�
    }
    #endregion
}
