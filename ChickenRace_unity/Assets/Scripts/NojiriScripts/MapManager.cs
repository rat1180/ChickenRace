using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ResorceNames;
using Dictionary;

/// <summary>
/// ��Q���̏��ێ��p�N���X
/// </summary>
[System.Serializable]
public class ObjectStatus
{
    public List<Vector2Int> childList; // �e�X�g�p
    public Dictionary_Unity<int, float> AngleList;        // <Key�F�ݒu����, Value�F�ݒu�������>
    public Dictionary_Unity<int, int> InstalledDic;       // <Key�F�ݒu����, Value�F��Q��id���>
    public Dictionary_Unity<Vector2Int, int> usedGridDic; // <Key�F�ݒu�ψʒu���, Value�F�ݒu����>
}

public class MapManager : MonoBehaviour
{
    //public float itemSize { get; set; } // �A�C�e���T�C�Y�ύX�p
    public float itemSize; // �A�C�e���T�C�Y�ύX�p
    public bool debugMode = false; // �f�o�b�O���[�h�t���O

    [SerializeField] private ObjectStatus objStatus; // ��Q���p�̍\���̏��

    private GameObject obstacleObj;      // �ړ��������I�u�W�F�N�g�̏��
    private GameObject gridObj;          // �O���b�h�\���p�I�u�W�F�N�g
    private GameObject panelObj;         // �O���b�h�\���p�p�l��
    private List<Vector2Int> childList;  // ��Q���̎q�I�u�W�F�N�g���X�g
    private Vector2 panelSize;           // �p�l���T�C�Y�ύX�p

    private bool isRunning = false; // �R���[�`�����s����t���O
    private bool isInstall = false; // �ݒu�t���O true�F�ݒu�\�@false�F�ݒu�s��
    private int installNum; // �u���ꂽ����

    // �e�X�g�p
    [SerializeField] private bool childTest = false;

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
        if (Input.GetKeyDown(KeyCode.C))
        {
            ReInstallObject();
        }
    }

    #region ��������
    /// <summary>
    /// �}�b�v���������\�b�h
    /// </summary>
    private void MapInit()
    {
        // ������
        objStatus.AngleList = new Dictionary_Unity<int, float>();
        objStatus.InstalledDic = new Dictionary_Unity<int, int>();
        objStatus.usedGridDic = new Dictionary_Unity<Vector2Int, int>();
        objStatus.childList = new List<Vector2Int>();     // �e�X�g
        installNum = 0;

        // �e�X�g�p
        if (debugMode)
        {
            // CollisionList���e�X�g�p
            objStatus.childList.Add(new Vector2Int(0, 1));
            objStatus.childList.Add(new Vector2Int(-1, 0));

            // �e�X�g�p
            //usedGridDic.Add(new Vector2Int(0, 0), 0);
            //usedGridDic.Add(new Vector2Int(0, 0), 0);
            //usedGridDic.Add(new Vector2Int(2, 0), 1);
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

    /// <summary>
    /// �N���G�C�g���[�h���̂݁A��Q���̐Î~���\��
    /// </summary>
    private void GenerateImage()
    {
        // �摜����
        for (int i = 0; i < installNum; i++)
        {
            // Value(id)�ɑΉ������摜�擾(���摜)
            //var image = ResourceManager.instance.GetObstacleImage((OBSTACLE_IMAGE_NAMES)i);
            GameObject imageObj = (GameObject)Resources.Load("shiitake");

            Debug.Log(imageObj);

            // i�Ɠ�����Value������Ƃ�
            if (objStatus.usedGridDic.ContainValue(i))
            {
                // �����ʒu�����X�g�Ŏ擾
                var key = objStatus.usedGridDic.GetKey(i);

                // �e�I�u�W�F�N�g�̐���
                var obj = Instantiate(imageObj, new Vector3(key.x, key.y), Quaternion.identity);
                obj.transform.parent = transform;
            }
        }
    }

    /// <summary>
    /// �q�I�u�W�F�N�g�̉摜��\�����\�b�h
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
        GenerateImage();

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
        isInstall = true;
        StopCoroutine(CreativeMode());
        GridDraw();
        DestroyImage();

        Debug.Log("�N���G�C�e�B�u���[�h�I��");
    }

    /// <summary>
    /// �ݒu���胁�\�b�h
    /// �ݒu�������I�u�W�F�N�g�̃O���b�h�ʒu���A�ݒu�\���ǂ�����߂�l�ŕԂ�
    /// ture�F�����\ / false�F�����s��
    /// </summary>
    /// <param name="installPos">�}�E�X�ʒu(�e�̐ݒu�ʒu)</param>
    /// <param name="id">�ݒu�������I�u�W�F�N�g�ԍ�</param>
    /// <returns></returns>
    public bool JudgeInstall(Vector2Int installPos, int id)
    {
        if (childTest)
        {
            // id�ɑΉ��������X�g���擾(��)
            childList = objStatus.childList;
        }
        else
        {
            // CollisionList���擾
            var obj = ResourceManager.instance.GetObstacleObject((OBSTACLE_OBJECT)id);
            childList = obj.GetComponent<Obstacle>().GetCollisionList();
        }

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
    /// ture�F�����\ / false�F�����s��
    /// </summary>
    /// <param name="pos">�}�E�X�ʒu(�e�̐ݒu�ʒu)</param>
    /// <returns></returns>
    private bool JudgeInstallCenter(Vector2Int pos)
    {
        // �ݒu�σ��X�g���ɁApos�Ɠ����ʒu��񂪂���Ƃ�
        if (objStatus.usedGridDic.ContainKey(pos)) return false;

        return true;
    }

    /// <summary>
    /// �ݒu����ʒu���胁�\�b�h
    /// �ݒu����I�u�W�F�N�g���Q�}�X�ȏ゠��ꍇ
    /// ture�F�����\ / false�F�����s��
    /// </summary>
    /// <param name="pos">�}�E�X�ʒu(�e�̐ݒu�ʒu)</param>
    /// <param name="list">�q�̐ݒu�ʒu���X�g</param>
    /// <returns></returns>
    private bool JudgeInstallChild(Vector2Int pos, List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var childInsPos = pos + list[i];

            // �ݒu�σ��X�g���ɁApos�Ɠ����ʒu��񂪂���Ƃ�
            if (objStatus.usedGridDic.ContainKey(childInsPos)) return false;
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

        if (!debugMode) // �I�t�̎�
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

        // ���������Q���̃T�C�Y�ύX
        obstacleObj.transform.localScale = new Vector3(itemSize, itemSize,1);

        // ��Q���̐���
        Instantiate(obstacleObj, new Vector3(gridPos.x, gridPos.y), Quaternion.Euler(0, 0, angle));

        // �ݒu�����I�u�W�F�N�gID��ǉ�
        objStatus.InstalledDic.Add(installNum, id);

        // �ݒu�����ʒu���Ɛݒu���Ԃ�ǉ�
        objStatus.usedGridDic.Add(gridPos, installNum);

        if (childList != null) // �q�I�u�W�F�N�g������Ƃ�
        {
            for (int i = 0; i < childList.Count; i++)
            {
                // �e�I�u�W�F�N�g�̍��W���q�I�u�W�F�N�g�̌����ǉ�
                objStatus.usedGridDic.Add(gridPos + childList[i], installNum);
            }
        }
        objStatus.AngleList.Add(installNum, angle);

        installNum++;
    }

    /// <summary>
    /// �폜�������s�p���\�b�h
    /// RemoveObstacle���\�b�h�ɂď�Q���폜��A�S�v���C���[���������\�b�h�����s
    /// </summary>
    /// <param name="id"></param>
    /// <param name="gridPos"></param>
    public void DeleteObject(Vector2Int gridPos)
    {
        if (!isRunning)
        {
            Debug.LogError("�N���G�C�e�B�u���[�h���J�n����Ă��܂���B");
            return;
        }

        // ��Q���폜
        RemoveObstacle(gridPos);

        if (!debugMode) // �I�t�̎�
        {
            // ���̃v���C���[��RemoveObstacle���\�b�h�̎��s
            var Obj = PhotonNetwork.Instantiate("GenerateObstacle", new Vector3(0, 0), Quaternion.identity);
            Obj.GetComponent<GenerateObstacle>().DeleteObstacle(gridPos);
        }
    }

    /// <summary>
    /// ��Q�����j�p���\�b�h
    /// �w�肳�ꂽ�}�X�ɏ�Q��������Ƃ��A���̏�Q�����폜
    /// �����}�X�������Ă����Q���̏ꍇ�A�w�肳�ꂽ�}�X�ȊO�̓����L�[����������Q�����폜
    /// </summary>
    /// <param name="deletePos">Vector2Int�^�̃}�E�X�ʒu�iTKey�j</param>
    public void RemoveObstacle(Vector2Int deletePos)
    {
        // �폜�������ʒu��񂩂�AValue(id)���擾
        var deleteNum = objStatus.usedGridDic.GetValue(deletePos);

        // �v�f�̍폜
        if (childList == null)
        {
            objStatus.usedGridDic.Remove(deletePos);
        }
        else
        {
            // �擾����Value(id)�ɑΉ�����Key���X�g���擾
            var keyList = objStatus.usedGridDic.GetKeyList(deleteNum);

            // Key���X�g�̗v�f���폜
            foreach (var list in keyList)
            {
                objStatus.usedGridDic.Remove(list);
            }
        }

        // Value(id)�ɑΉ������A���ꂼ��̃��X�g�v�f���폜
        objStatus.InstalledDic.Remove(deleteNum);
        objStatus.AngleList.Remove(deleteNum);
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


        // �Đ���
        for (int i = 0; i < installNum; i++)
        {
            // Key�ɑΉ�����Value(id)�̃I�u�W�F�N�g�擾
            obstacleObj = GetObstaclePrefab(objStatus.InstalledDic.GetValue(i));

            // i�Ɠ�����Value������Ƃ�
            if (objStatus.usedGridDic.ContainValue(i))
            {
                var key = objStatus.usedGridDic.GetKey(i);   // �����ʒu�����X�g�Ŏ擾
                var angle = objStatus.AngleList.GetValue(i); // �����p�x���擾
                Debug.Log(angle);

                // �e�I�u�W�F�N�g�̐���
                Instantiate(obstacleObj, new Vector3(key.x, key.y), Quaternion.Euler(0, 0, angle));
            }
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

    /// <summary>
    /// �O���b�h�̃T�C�Y���ύX���ꂽ�Ƃ��A�A�C�e���̑傫�����O���b�h�T�C�Y�ɍ��킹��
    /// </summary>
    /// <param name="size">�ύX��̃O���b�h�T�C�Y</param>
    //public void ChangeItemSize(float size)
    //{
    //    itemSize = size;
    //}

    // �e�X�g
    //public void GetGridSize(Vector2 gridSize)
    //{
    //    // �T�C�Y�ς���
    //    panelSize = gridSize;
    //}
    #endregion
}
