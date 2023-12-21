using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ResorceNames;

/// <summary>
/// ��Q���̏��ێ��p�N���X
/// </summary>
[System.Serializable]
public struct ObjectStatus
{
    public List<int> InstalledList;        // �ݒu������Q��id���X�g
    public List<float> AngleList;          // ��Q���̐ݒu�������X�g
    public List<Vector2Int> UsedGridList;  // �g�p�ς݃O���b�h�̈ʒu���X�g
    public List<Vector2Int> testList; // �e�X�g�p
}

public class MapManager : MonoBehaviour
{
    public bool debugMode = false; // �f�o�b�O���[�h�t���O

    [SerializeField] private ObjectStatus objStatus; // ��Q���p�̍\���̏��
    private GameObject obstacleObj; // �ړ��������I�u�W�F�N�g�̏��擾
    private GameObject gridObj;
    private GameObject panelObj;
    private List<Vector2Int> childList;
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
        objStatus.testList = new List<Vector2Int>();     // �e�X�g
        obstacleObj = new GameObject();

        // �e�X�g�p
        if (debugMode)
        {
            objStatus.testList.Add(new Vector2Int(0, 1));
            objStatus.testList.Add(new Vector2Int(-1, 0));
        }

        // �O���b�h�ƃp�l���̏����擾
        gridObj = Instantiate((GameObject)Resources.Load("GridObject"));
        panelObj = GameObject.Find("CanvasUI/GridPanel");
        panelSize = panelObj.transform.GetComponent<RectTransform>().sizeDelta;

        // �����͔�\��
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
        var obj = ResourceManager.instance.GetObstacleObject((OBSTACLE_OBJECT)id);

        return obj;
    }

    /// <summary>
    /// �O���b�h�`��ؑփ��\�b�h
    /// �N���G�C�e�B�u���[�h�̏�Ԃɂ���Đ؂�ւ�
    /// </summary>
    private void GridDraw()
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
    /// �ݒu�������I�u�W�F�N�g�̃O���b�h�ʒu���A�ݒu�\���ǂ�����߂�l�ŕԂ�
    /// ture�F�����\ / false�F�����s��
    /// </summary>
    /// <param name="installPos">�}�E�X�̃O���b�h�ʒu</param>
    /// <param name="id">�ݒu�������I�u�W�F�N�g�ԍ�</param>
    /// <returns></returns>
    public bool JudgeInstall(Vector2Int installPos, int id)
    {
        // id�ɑΉ��������X�g���擾
        childList = objStatus.testList;
        //var Obj = ResourceManager.instance.GetObstacleObject((OBSTACLE_OBJECT)id);
        //childList = Obj.GetComponent<Obstacle>().Seter();

        // �ݒu�ʒu����̂Ƃ�
        if (childList == null)
        {
            return JudgeInstallCenter(installPos);
        }
        else // ��łȂ���
        {
            // �S�Ẵ}�X�Őݒu�\���ǂ���
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

    // �����\��
    public bool JudgeInstall(Vector2Int installPos)
    {
        return true;
    }

    /// <summary>
    /// �ݒu����ʒu���胁�\�b�h
    /// �ݒu����I�u�W�F�N�g���P�}�X�̏ꍇ
    /// </summary>
    /// <param name="pos">�}�E�X�̃O���b�h�ʒu</param>
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
    /// �ݒu����ʒu���胁�\�b�h
    /// �ݒu����I�u�W�F�N�g���Q�}�X�ȏ゠��ꍇ
    /// </summary>
    /// <param name="pos">�}�E�X�̃O���b�h�ʒu</param>
    /// <param name="list">pos�ȊO�̈ʒu���X�g</param>
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

        if (!debugMode)
        {
            // ���̃v���C���[��SpawnObstacle���\�b�h�̎��s
            var Obj = PhotonNetwork.Instantiate("GenerateObstacle", new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));
            Obj.GetComponent<GenerateObstacle>().SetObstacleID(id, angle, gridPos);
        }

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

    /// <summary>
    /// �t���O�Q�ƃ��\�b�h
    /// </summary>
    /// <returns>�ݒu�t���O�@true:�ݒu�ς݁@false:���ݒu</returns>
    public bool IsInstallReference()
    {
        return isInstall;
    }

    // �e�X�g
    //public void GetGridSize(Vector2 gridSize)
    //{
    //    // �T�C�Y�ς���
    //    panelSize = gridSize;
    //}
    #endregion
}
