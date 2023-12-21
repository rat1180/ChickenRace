using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
  �f�o�b�O�p�̃}�l�[�W���[
�@�ʏ�A�g���邱�Ƃ�z�肵�Ă��Ȃ����߁A
�@�e�}�l�[�W���[���̎Q�ƃe�X�g�ł̂ݎg�p���邱��
 */


public class DebugManager : GameManager
{
    GameProgress gameProgress;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        gameProgress = new GameProgress();
        instance = this;

        //User�N���X����
        var user_class = Instantiate((GameObject)Resources.Load("User"), Vector3.zero, Quaternion.identity);
        gameProgress.user = user_class.GetComponent<User>();

        //MapManager�𐶐�
        var map_class = Instantiate((GameObject)Resources.Load("MapManager"), Vector3.zero, Quaternion.identity);
        gameProgress.mapManager = map_class.GetComponent<MapManager>();

        //UIManager������
        gameProgress.uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
}
