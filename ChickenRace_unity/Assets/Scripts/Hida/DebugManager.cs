using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
  デバッグ用のマネージャー
　通常、使われることを想定していないため、
　各マネージャー等の参照テストでのみ使用すること
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

        //Userクラス生成
        var user_class = Instantiate((GameObject)Resources.Load("User"), Vector3.zero, Quaternion.identity);
        gameProgress.user = user_class.GetComponent<User>();

        //MapManagerを生成
        var map_class = Instantiate((GameObject)Resources.Load("MapManager"), Vector3.zero, Quaternion.identity);
        gameProgress.mapManager = map_class.GetComponent<MapManager>();

        //UIManagerを検索
        gameProgress.uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
}
