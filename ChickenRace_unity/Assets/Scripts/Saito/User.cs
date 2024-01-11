using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject mouse;
    int isMode;
    int modeId;
    int itemId;
    [SerializeField] int index;

    //��
    GameObject mouseObjcet;
    GameObject playerObjcet;

    GameManager gameManager;

    /// <summary>
    /// �������p�֐�.
    /// </summary>
    void Init()
    {
        isMode = 0;
        modeId = 0;
        itemId = -1;
        index = -1;
        gameManager = GameManager.instance;
    }

    private void Awake()
    {
        // GeneratePlayer();
    }

    void Start()
    {
        Init();
    }


    void Update()
    {
        PlayerGoal();
    }

    public void GeneratePlayer()
    {
        var obj = Instantiate(player, transform.position, transform.rotation);
        playerObjcet = obj;
        playerObjcet.GetComponent<Player>().ImageInstance();
    }

    /// <summary>
    /// �}�E�X�̐���.
    /// </summary>
    /// <param name="mode"></param>
    public void GenerateMouse(int mode)
    {
        mouseObjcet = Instantiate(mouse, transform.position, transform.rotation);
        mouseObjcet.GetComponent<PlayerMouse>().SetUser(this);

        // �I���t�F�[�Y.
        if (mode == 0)
        {
            Init();
            GetModeId();
        }
        // �ݒu�t�F�[�Y.
        else
        {
            GetModeId(mode);
            mouseObjcet.GetComponent<PlayerMouse>().PlantPhase();
        }

    }

    /// <summary>
    /// �}�E�X���폜.
    /// </summary>
    public void DestroyMouse()
    {
        Destroy(mouseObjcet);
    }

    /// <summary>
    /// �v���C���[���폜.
    /// </summary>
    public void DestroyPlayer()
    {
        Destroy(playerObjcet);
    }

    public int SetMode()
    {
        return isMode;
    }

    /// <summary>
    /// GenerateMouse��������̒l���󂯎��Ȃ��������ɌĂ�.
    /// </summary>
    void GetModeId()
    {
        modeId = -1;
    }

    /// <summary>
    /// GenerateMouse��������̒l���󂯎�������ɌĂ�.
    /// </summary>
    /// <param name="mode"></param>
    private void GetModeId(int mode)
    {
        modeId = isMode;
    }

    public void SetItemId()
    {
        itemId = -1;
    }

    public void SetItemId(int itemid)
    {
        itemId = itemid;
    }

    public int GetItemId()
    {
        return itemId;
    }

    /// <summary>
    /// �C���f�b�N�X���󂯎��֐�.
    /// </summary>
    /// <param name="setindex"></param>
    public void SetIndex(int setindex)
    {
        index = setindex;
    }

    public int GetIndex()
    {
        return index;
    }

    public void PlayerStart(bool isstart)
    {
        if (playerObjcet != null)
        {
            playerObjcet.GetComponent<Player>().IsStart(isstart);
        }
    }

    public void PlayerGoal()
    {
        if (playerObjcet != null)
        {
            if (playerObjcet.GetComponent<Player>().GoalCheck())
            {
                gameManager.GoalPlayer();
            }
        }
    }

    public void StartPlayerPosition(Vector3 startpos)
    {
        playerObjcet.GetComponent<Player>().StartPosition(startpos);
    }
}
