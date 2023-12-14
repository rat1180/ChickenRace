using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject mouse;
    int isMode;
    int itemId;

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

        Instantiate(mouse, transform.position, transform.rotation);
    }

    /// <summary>
    /// マウスを削除.
    /// </summary>
    public void DestroyMouse()
    {
        Destroy(gameObject);
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

    }

    /// <summary>
    /// GenerateMouseから引数の値を受け取った時に呼ぶ.
    /// </summary>
    /// <param name="mode"></param>
    private void GetItemId(int mode)
    {
        itemId = isMode;

    }
}
