using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject mouse;
    int isMode;
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

    public void GenerateMouse(int mode)
    {
        if(mode == 0)
        {
            GetItemId();
        }
        else
        {
            GetItemId(mode);
        }

        mouseObjcet = Instantiate(mouse, transform.position, transform.rotation);
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
    void GetItemId()
    {
        itemId = -1;
    }

    /// <summary>
    /// GenerateMouseから引数の値を受け取った時に呼ぶ.
    /// </summary>
    /// <param name="mode"></param>
    private void GetItemId(int mode)
    {
        itemId = isMode;
    }

    public void GetIndex(int getindex)
    {
        index = getindex;
    }
}
