using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject mouse;
    int isMode;
    int index;

    //��
    GameObject mouseObjcet;

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
    private void GetItemId(int mode)
    {
        index = isMode;

    }
}
