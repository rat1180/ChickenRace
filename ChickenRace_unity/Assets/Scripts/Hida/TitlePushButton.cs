using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
  �^�C�g������ړ�����p�̃X�N���v�g
 */

public class TitlePushButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushButton()
    {
        SceneManager.LoadScene("Loby");
    }
}
