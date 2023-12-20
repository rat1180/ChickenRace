using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// ��Q���̏��ێ��p�N���X
/// </summary>
[System.Serializable]
public struct ObjectStatus
{
    public List<int> InstalledList;        // �ݒu������Q��id���X�g
    public List<float> AngleList;          // ��Q���̐ݒu�������X�g
    public List<Vector2Int> UsedGridList;  // �g�p�ς݃O���b�h�̈ʒu���X�g
}

public class MapManager : MonoBehaviour
{
    [SerializeField] private ObjectStatus objStatus; // ��Q���p�̍\���̏��
    [SerializeField] private GameObject obstacleObj; // �ړ��������I�u�W�F�N�g�̏��擾
    [SerializeField] private GameObject panelObject; // �O���b�h�p�p�l��
    private GameObject gridObj;
    private GameObject panelObj;
    private Vector2 panelSize;

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
        // ������
        objStatus.InstalledList = new List<int>();
        objStatus.AngleList = new List<float>();
        objStatus.UsedGridList = new List<Vector2Int>();
        obstacleObj = new GameObject();

        gridObj = Instantiate((GameObject)Resources.Load("GridObject"));
        panelObj = GameObject.Find("CanvasUI/GridPanel");
        panelSize = panelObj.transform.GetComponent<RectTransform>().sizeDelta;

        gridObj.SetActive(false);
        panelObj.SetActive(false);
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
            �R���[�`�����s��
            �E�ݒu����@�@�@�FJudgeInstall()
            �E��Q�������@�@�FGenerateMapObject()�ASpawnObstacle()
            �E�R���[�`���I���FCreativeModeEnd()

            �R���[�`�������s��
            �E��Q���Ĕz�u�@�FReInstallObject()
            �E�R���[�`���J�n�FCreativeModeStart()

            ��L�̃��\�b�h���Ăяo����
            */

            yield return null;
        }
    }

    /// <summary>
    /// ResourcesManager����id�ɑΉ�����I�u�W�F�N�g���擾����
    /// </summary>
    private GameObject GetObstaclePrefab(int id)
    {
        return obstacleObj;
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
            Debug.LogError("�N���G�C�e�B�u���[�h���s���ł��B");
            return;
        }

        isRunning = true;
        isInstall = false;
        StartCoroutine(CreativeMode());
        GridDraw();

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
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B");
            return;
        }

        isRunning = false;
        StopCoroutine(CreativeMode());
        GridDraw();

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
        for (int i = 0; i < objStatus.UsedGridList.Count; i++)
        {
            if (installPos == objStatus.UsedGridList[i])
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// �S�v���C���[��Q���������\�b�h
    /// ID�A���������A�O���b�h�ʒu���擾
    /// SpawnObstacle���\�b�h�ɂď�Q��������A�S�v���C���[���������\�b�h�����s
    /// </summary>
    /// <param name="id">��������I�u�W�F�N�g�ԍ�</param>
    /// <param name="angle">��������ۂ̌���</param>
    /// <param name="gridPos">��������ʒu</param>
    public void GenerateMapObject(int id, float angle, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B");
            return;
        }

        // ��Q������
        SpawnObstacle(id, angle, gridPos);

        // ���̃v���C���[��SpawnObstacle���\�b�h�̎��s
        var Obj = PhotonNetwork.Instantiate("GenerateObstacle", new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));
        Obj.GetComponent<GenerateObstacle>().SetObstacleID(id, angle, gridPos);

        isInstall = true;
    }

    /// <summary>
    /// ��Q���������\�b�h
    /// </summary>
    /// <param name="id">��������I�u�W�F�N�g�ԍ�</param>
    /// <param name="angle">��������ۂ̌���</param>
    /// <param name="gridPos">��������ʒu</param>
    public void SpawnObstacle(int id, float angle, Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B");
            return;
        }

        // ��Q���̎擾
        obstacleObj = GetObstaclePrefab(id);

        // ��Q���̐���
        Instantiate(obstacleObj, new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));

        // �ݒu�����I�u�W�F�N�gID�ƈʒu�����X�g�ɒǉ�
        objStatus.InstalledList.Add(id);
        objStatus.UsedGridList.Add(gridPos);
        objStatus.AngleList.Add(angle);
    }

    /// <summary>
    /// �t���O�Q�ƃ��\�b�h
    /// </summary>
    /// <returns>�ݒu�t���O�@true:�ݒu�ς݁@false:���ݒu</returns>
    public bool IsInstallReference()
    {
        return isInstall;
    }

    /// <summary>
    /// ��Q���̍Đݒu���\�b�h
    /// ���E���h�I�����A��Q�����Đݒu���邱�Ƃŏ�����Ԃɖ߂�
    /// </summary>
    public void ReInstallObject()
    {
        if (isRunning)
        {
            Debug.Log("�N���G�C�e�B�u���[�h�����s���̂��߁A�Ĕz�u�ł��܂���B");
            return;
        }

        for (int i = 0; i < objStatus.InstalledList.Count; i++)
        {
            obstacleObj = GetObstaclePrefab(objStatus.InstalledList[i]);
            Instantiate(obstacleObj, new Vector3(objStatus.UsedGridList[i].x, objStatus.UsedGridList[i].y), Quaternion.Euler(0, 0, objStatus.AngleList[i]));
        }
    }

    public void GridDraw()
    {
        if (!isRunning)
        {
            Debug.Log("�O���b�h�\�����ł��Ȃ���Ԃł��B");
            gridObj.SetActive(false);
            panelObj.SetActive(false);
            return;
        }

        gridObj.SetActive(true);
        panelObj.SetActive(true);

        //panelObj.transform.localScale = new Vector3(0.1f, 0.1f, 1f);
    }

    // �e�X�g
    //public void GetGridSize(Vector2 gridSize)
    //{
    //    // �T�C�Y�ς���
    //    panelSize = gridSize;
    //}
    #endregion
}
