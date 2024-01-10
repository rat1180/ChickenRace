using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
  タイトルから移動する用のスクリプト
 */

public class TitlePushButton : MonoBehaviour
{
    bool isSceanMove;
    // Start is called before the first frame update
    void Start()
    {
        isSceanMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isSceanMove)
        {
            isSceanMove = true;
            PushButton();
        }
    }

    public void PushButton()
    {
        SceneManager.LoadScene("Assets/Scenes/NagatsukaScenes/Lobby.unity");
    }
}
