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

    //��
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
    /// �}�E�X���폜.
    /// </summary>
    public void DestroyMouse()
    {
        Destroy(mouseObjcet);
        //Destroy(gameObject);
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
    void GetItemId()
    {
        itemId = -1;
    }

    /// <summary>
    /// GenerateMouse��������̒l���󂯎�������ɌĂ�.
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
