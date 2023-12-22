using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
  タイトルから移動する用のスクリプト
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
