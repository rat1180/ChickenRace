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

    //仮
    GameObject mouseObjcet;
    GameObject playerObjcet;

    GameManager gameManager;

    /// <summary>
    /// 初期化用関数.
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
    /// マウスの生成.
    /// </summary>
    /// <param name="mode"></param>
    public void GenerateMouse(int mode)
    {
        mouseObjcet = Instantiate(mouse, transform.position, transform.rotation);
        mouseObjcet.GetComponent<PlayerMouse>().SetUser(this);

        // 選択フェーズ.
        if (mode == 0)
        {
            Init();
            GetModeId();
        }
        // 設置フェーズ.
        else
        {
            GetModeId(mode);
            mouseObjcet.GetComponent<PlayerMouse>().PlantPhase();
        }

    }

    /// <summary>
    /// マウスを削除.
    /// </summary>
    public void DestroyMouse()
    {
        Destroy(mouseObjcet);
    }

    /// <summary>
    /// プレイヤーを削除.
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
    /// GenerateMouseから引数の値を受け取らなかった時に呼ぶ.
    /// </summary>
    void GetModeId()
    {
        modeId = -1;
    }

    /// <summary>
    /// GenerateMouseから引数の値を受け取った時に呼ぶ.
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
    /// インデックスを受け取る関数.
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
