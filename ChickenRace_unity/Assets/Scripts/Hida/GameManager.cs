using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PhotonMethods;
using ResorceNames;

public class GameManager : MonoBehaviour
{
    enum GameStatus
    {
        SLEEP,
        READY,  //開始準備中
        START,  //ゲーム開始前
        SELECT, //障害物選択中
        PLANT,  //障害物設置中
        RACE,   //レース中
        RESULT, //スコア表示、反映中
        END     //ゲーム終了
    }

    //初期化の段階を示す
    //その段階が終わると次の状態に進む
    enum InitStatus
    {
        CONECT,  //接続中
        RESET,   //初期化中
        WAIT,    //他プレイヤー待機
        START,   //ゲーム開始可能
    }

    enum InGameStatus
    {
        NONE,
        READY,
        INGAME,
        END,
    }

    /// <summary>
    /// ゲームの進行に必要なマネージャー等をまとめたクラス
    /// </summary>
    [System.Serializable]
    public class GameProgress
    {
        public MapManager mapManager;
        public UIManager uiManager;
        public DataSharingClass dataSharingClass;
        public User user;
    }

    [SerializeField, Tooltip("現在のゲーム状態")] GameStatus gameState;
    [SerializeField, Tooltip("進行中のステートコルーチン")] Coroutine stateCoroutine;
    [SerializeField, Tooltip("他のクラスから渡される、現在のフェーズ終了を知らせる変数")] bool isFazeEnd; //Int型にして複数の状態に対応出来るようにするかも
    [SerializeField, Tooltip("ゲームの進行に必要なクラスのまとめ")] GameProgress gameProgress;
    [SerializeField, Tooltip("デバッグ用のログを表示するかどうか")] bool isDebug;


    public static GameManager instance;

    #region Unityイベント

    // Start is called before the first frame update
    void Start()
    {
        gameState = GameStatus.SLEEP;
        StartCoroutine(GameInit());
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();

        if(Input.GetKeyDown(KeyCode.RightArrow)){
            DebugNextState();
        }
    }

    #endregion

    #region 関数

    #region クラス内で使用する関数

    /// <summary>
    /// 各プレイヤーの初期化状態を確認する
    /// 全員が引数の状態の時にtrueを返す
    /// この初期化状態はカスタムプロパティで管理する
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    bool CheckInitState(InitStatus status)
    {
        //全員の初期化状態を確認
        foreach(var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.GetInitStatus() != (int)status) return false;
        }
        return true;
    }

    bool CheckInGameState(InGameStatus status)
    {
        //全員の初期化状態を確認
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            if (player.Value.GetInGameStatus() != (int)status) return false;
        }
        return true;
    }

    /// <summary>
    /// ステートコルーチンをクリアする
    /// ステートコルーチン終了時に必ず呼ぶこと
    /// </summary>
    void ClearCoroutine()
    {
        StopCoroutine(stateCoroutine);

        stateCoroutine = null;
        isFazeEnd = false;
    }


    /// <summary>
    /// 対象のリストからIndexの値を取得する
    /// リストがセットされていないときはエラーとして-1を返す
    /// </summary>
    /// <param name="id_list"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    int CheckObstacleIndex(List<int> id_list, int index)
    {
        if (id_list.Count == 0) return -1;

        return id_list[index];
    }

    /// <summary>
    /// Userが選択を終え、正常にアイテムを持っているかを確認する
    /// </summary>
    /// <returns></returns>
    bool CheckUserIsHave()
    {
        //正常に持っているか確認
        if (/*gameProgress.user. != -1*/false)
        {
            PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);
            return true;
        }
        return false;
    }

    #endregion

    #region クラス外で使用する関数

    /// <summary>
    /// フェーズに関わるクラスで、
    /// そのフェーズ内で行う必要のある処理を終えた場合呼ぶ
    /// 例：設置完了、キャラヒットで死亡
    /// </summary>
    public void EndFaze()
    {
        isFazeEnd = true;
    }

    /// <summary>
    /// 障害物選択フェーズでマウスが選択したオブジェクトを取得する
    /// リストに対応するIndexをもらい、リスト内が0(選択済み)でない場合に選択確定とする
    /// 対応するIDが0の時はfalse,0出ないときはtrueを返す
    /// </summary>
    /// <param name="id"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool MouseSelected(int index)
    {
        var id = CheckObstacleIndex(gameProgress.dataSharingClass.ID, index);
        if (id == -1 || id == 0) return false;

        gameProgress.dataSharingClass.ResetID(index);
        //gameProgress.user = id;
        return true;
    }

    public void DeadPlayer()
    {
        EndFaze();
    }

    public void SetDataSheringClass(DataSharingClass datasharingclass)
    {
        gameProgress.dataSharingClass = datasharingclass;
    }

    public GameObject GetMapManager()
    {
        return gameProgress.mapManager.gameObject;
    }

    #endregion

    #region デバッグ用

    void DebugLog(string message)
    {
        if (isDebug) Debug.Log(message);
    }

    void DebugLogWarning(string message)
    {
        if (isDebug) Debug.LogWarning(message);
    }

    void DebugNextState()
    {
        gameState++;
        ClearCoroutine();
    }

    #endregion

    /// <summary>
    /// 現在のgameStateに応じて適切なステートコルーチンを呼び出す
    /// ステートコルーチンは終了時にコルーチン内で破棄され、
    /// 実行中出ないときは常にNULLになる
    /// </summary>
    void GameLoop()
    {
        //フェーズ中
        if (stateCoroutine != null) return;

        string coroutinename = "null";
        switch (gameState)
        {
            case GameStatus.SLEEP:
                return;
                break;
            case GameStatus.READY:
                coroutinename = "StateREADY";
                break;
            case GameStatus.START:
                coroutinename = "StateSTART";
                break;
            case GameStatus.SELECT:
                coroutinename = "StateSELECT";
                break;
            case GameStatus.PLANT:
                coroutinename = "StatePLANT";
                break;
            case GameStatus.RACE:
                coroutinename = "StateRACE";
                break;
            case GameStatus.RESULT:
                coroutinename = "StateRESULT";
                break;
            case GameStatus.END:
                break;
        }
        if(coroutinename == "null")
        {
            DebugLogWarning("コルーチンが正常に振り分けられていません");
            return;
        }
        DebugLog(coroutinename);
        stateCoroutine = StartCoroutine(coroutinename);
    }

    #endregion

    #region コルーチン

    IEnumerator GameInit()
    {
        //自分自身を取得
        var localplayer = PhotonNetwork.LocalPlayer;
        InitStatus initStatus = InitStatus.CONECT;

        //1.接続を確認<CONECT
        {
            //フォトンの機能で接続しているか確認
            while (!PhotonNetwork.InRoom)
            {
                //接続まで待機
                DebugLog("接続確認中..");
                yield return null;
            }

            //状態を送信
            initStatus = InitStatus.CONECT;
            localplayer.SetInitStatus((int)initStatus);

            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckInitState(InitStatus.CONECT));
            DebugLog("接続確認!");

        }

        //マスターの開始を待機
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("開始まで待機");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.S));
            //状態を送信
            initStatus = InitStatus.RESET;
            localplayer.SetInitStatus((int)initStatus);
        }
        else
        {
            Debug.Log("開始まで待機");
            yield return new WaitUntil(() => PhotonNetwork.MasterClient.GetInitStatus() == (int)InitStatus.RESET);
            //状態を送信
            initStatus = InitStatus.RESET;
            localplayer.SetInitStatus((int)initStatus);
        }

        //他のプレイヤーを待機
        yield return new WaitUntil(() => CheckInitState(InitStatus.RESET));

        //2.各値を初期化<RESET
        {
            gameProgress = new GameProgress();
            gameState = GameStatus.READY;
            isFazeEnd = false;
            stateCoroutine = null;
            instance = this;

            //各マネージャーを生成
            //データ共有クラスを生成
            if (PhotonNetwork.LocalPlayer.IsMasterClient) //ホスト
            {
                //データ共有クラスを生成
                var obj = PhotonNetwork.Instantiate("NagatsukaObjects/DataSharingClass", Vector3.zero, Quaternion.identity);//データ共有クラスを生成する.
            }
            else                                          //ゲスト
            {
                //データ共有クラスが生成されるまで待機
                yield return new WaitUntil(() => gameProgress.dataSharingClass != null);
            }

            //Userクラス生成
            var user_class = Instantiate((GameObject)Resources.Load("User"), Vector3.zero, Quaternion.identity);
            gameProgress.user = user_class.GetComponent<User>();

            //MapManagerを生成
            var map_class = Instantiate((GameObject)Resources.Load("MapManager"), Vector3.zero, Quaternion.identity);
            gameProgress.mapManager = map_class.GetComponent<MapManager>();

            //UIManagerを検索
            gameProgress.uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

            //状態を送信
            initStatus = InitStatus.WAIT;
            localplayer.SetInitStatus((int)initStatus);
            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckInitState(InitStatus.WAIT));
        }
        //3.初期化完了・他プレイヤーを待機<WAIT
        {
            //同時にゲーム開始
            //状態を送信
            initStatus = InitStatus.START;
            localplayer.SetInitStatus((int)initStatus);
            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckInitState(InitStatus.START));
        }
        DebugLog("初期化完了");
        gameState = GameStatus.START;
    }

    #region ステートコルーチン

    /// <summary>
    /// READY状態の時に呼ばれるコルーチン
    /// Init内でstateがSTARTに変化するまで待機し、
    /// その間ユーザーにロード画面を表示する
    /// </summary>
    /// <returns></returns>
    IEnumerator StateREADY()
    {
        while(gameState != GameStatus.START)
        {
            //準備中であることを表示

            yield return null;
        }

        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    /// <summary>
    /// START状態の時に呼ばれるコルーチン
    /// ゲーム開始直前に演出や確認を行う
    /// UIManagerに演出を要請する
    /// </summary>
    /// <returns></returns>
    IEnumerator StateSTART()
    {
        //ゲーム開始前に行う処理・演出を行う
        DebugLog("スタート前表示");
        //UIManagerの演出終了によって終了呼び出し
        while (!isFazeEnd)
        {
            DebugLog("演出中...");
            yield return new WaitForSeconds(3.0f);
            EndFaze();
            yield return null;
        }
        DebugLog("ゲームスタート");
        gameState++;
        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    /// <summary>
    /// SELECT状態の時に呼ばれるコルーチン
    /// 障害物選択のためにUserクラスに選択状態を指示し、
    /// 障害物選択画面を表示する
    /// </summary>
    /// <returns></returns>
    IEnumerator StateSELECT()
    {
        DebugLog("障害物選択開始");

        //ホストなら障害物を抽選
        if (PhotonNetwork.IsMasterClient)
        {
            //テスト
            for (int i = 0; i < 4; i++)
            {
                int id = Random.Range(1, 4);
                //障害物追加
                gameProgress.dataSharingClass.PushID(i == 3 ? 0 : id);
            }
        }
        //ゲストなら抽選まで待機
        else
        {
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID[gameProgress.dataSharingClass.ID.Count - 1] == 0);
        }

        //選択クラスを生成
        gameProgress.user.GenerateMouse(0);
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.READY);

        //選択クラスによって終了呼び出し
        while (!isFazeEnd)
        {
            //障害物候補を表示
            List<OBSTACLE_IMAGE_NAMES> list = new List<OBSTACLE_IMAGE_NAMES>();

            foreach(var id in gameProgress.dataSharingClass.ID)
            {
                list.Add((OBSTACLE_IMAGE_NAMES)id);
            }

            gameProgress.uiManager.PushID(list);

            //時間制限にかかれば終了
            if (false)
            {
                //マウス削除
                gameProgress.user.DestroyMouse();

                //仮のアイテム渡し
                {
                    int random;
                    while (true)
                    {
                        random = Random.Range(0, gameProgress.dataSharingClass.ID.Count - 1);
                        if (gameProgress.dataSharingClass.ID[random] != 0) break;
                    }

                    //gameProgress.user. = gameProgress.dataSharingClass.ID[random];
                }
            }

            //マウスから障害物情報が送られ、userに渡れば終了
            if (CheckUserIsHave())
            {
                EndFaze();
            }

            //テスト用
            if (Input.GetKeyDown(KeyCode.S)) EndFaze();

            yield return null;
        }

        //マウス削除
        gameProgress.user.DestroyMouse();

        //状態送信
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);

        //全員の障害物選択まで待機
        yield return new WaitUntil(() => CheckInGameState(InGameStatus.END));

        DebugLog("障害物選択終了");
        gameState++;

        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    /// <summary>
    /// PLANT状態の時に呼ばれるコルーチン
    /// 障害物設置のためにUserクラスに設置状態を指示し、
    /// 障害物設置画面を表示する
    /// </summary>
    /// <returns></returns>
    IEnumerator StatePLANT()
    {
        DebugLog("障害物設置開始");
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.READY);
        gameProgress.mapManager.CreativeModeStart();

        //マウス生成
        gameProgress.user.GenerateMouse(1);

        while (!isFazeEnd)
        {
            //設置中
            //設置されたかどうかをmapManagerから受取
            if (gameProgress.mapManager.IsInstallReference())
            { 
                EndFaze();
            }

            //時間切れで設置終了
            if (false)
            {
                EndFaze();
            }


            yield return null;
        }

        //設置終了指示
        gameProgress.mapManager.CreativeModeEnd();
        //マウス削除
        gameProgress.user.DestroyMouse();

        //状態送信
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);

        //全員の障害物選択まで待機
        yield return new WaitUntil(() => CheckInGameState(InGameStatus.END));

        DebugLog("障害物設置終了");
        gameState++;

        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    /// <summary>
    /// RACE状態の時に呼ばれるコルーチン
    /// キャラが全て終了するまで待機し、
    /// 終了時に結果をまとめる
    /// </summary>
    /// <returns></returns>
    IEnumerator StateRACE()
    {
        DebugLog("レースフェーズ開始");
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.READY);

        //全員が待機状態になるまで待機
        yield return new WaitUntil(() => CheckInGameState(InGameStatus.READY));

        //キャラの出現
        gameProgress.user.GeneratePlayer();

        DebugLog("READY演出");

        DebugLog("レーススタート");
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.INGAME);
        //キャラの操作のロックを解除

        //プレイ中の演出
        while (!isFazeEnd)
        {
            //レース中の表示、演出

            DebugLog("レース中");

            yield return null;
        }

        //自身の状態を送信
        PhotonNetwork.LocalPlayer.SetInGameStatus((int)InGameStatus.END);

        while (!CheckInGameState(InGameStatus.END))
        {
            //死亡後、ゴール後の観戦

            yield return null;
        }

        //スコアの送信

        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    IEnumerator StateRESULT()
    {
        while (true)
        {
            yield return null;
        }
        
    }

    #endregion

    #endregion
}
