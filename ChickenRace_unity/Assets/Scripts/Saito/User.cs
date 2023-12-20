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

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    public void GeneratePlayer()
    {
        Instantiate(player, transform.position, transform.rotation);
    }

    /// <summary>
    /// マウスの生成.
    /// </summary>
    /// <param name="mode"></param>
    public void GenerateMouse(int mode)
    {
        if(mode == 0)
        {
            GetModeId();
        }
        else
        {
            GetModeId(mode);
        }

        mouseObjcet = Instantiate(mouse, transform.position, transform.rotation);
        mouseObjcet.GetComponent<PlayerMouse>().SetUser(this);
    }

    /// <summary>
    /// マウスを削除.
    /// </summary>
    public void DestroyMouse()
    {
        Destroy(mouseObjcet);
        //Destroy(gameObject);
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
}
