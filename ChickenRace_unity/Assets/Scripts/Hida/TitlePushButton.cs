using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PhotonMethods;

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
        SoundManager.instance.PlayBGM(SoundName.BGMCode.BGM_TITLE);
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
        Photon.Pun.PhotonNetwork.LocalPlayer.SetCharColorStatus(Random.Range(0, 3));
        SceneManager.LoadScene("Assets/Scenes/NagatsukaScenes/Lobby.unity");
    }
}
