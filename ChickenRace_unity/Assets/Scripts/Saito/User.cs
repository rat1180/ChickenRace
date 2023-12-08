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

    public int SetMode()
    {
        return isMode;
    }

    /// <summary>
    /// GenerateMouse��������̒l���󂯎��Ȃ��������ɌĂ�.
    /// </summary>
    void GetItemId()
    {

    }

    /// <summary>
    /// GenerateMouse��������̒l���󂯎�������ɌĂ�.
    /// </summary>
    /// <param name="mode"></param>
    void GetItemId(int mode)
    {
        itemId = isMode;

    }
}
