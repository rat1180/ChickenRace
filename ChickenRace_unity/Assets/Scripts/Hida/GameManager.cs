using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using PhotonMethods;
using ResorceNames;
using ConstList;
using UnityEngine.SceneManagement;

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

    const float DEAD = -1f;
    const int BONUS_SCORE = 1;
    const int BASE_SCORE = 3;
    const int GAME_END_SCORE = 10;

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
        public int userActorNumber;
    }

    [SerializeField, Tooltip("現在のゲーム状態")] GameStatus gameState;
    [SerializeField, Tooltip("進行中のステートコルーチン")] Coroutine stateCoroutine;
    [SerializeField, Tooltip("他のクラスから渡される、現在のフェーズ終了を知らせる変数")] bool isFazeEnd; //Int型にして複数の状態に対応出来るようにするかも
    [SerializeField, Tooltip("ゲームの進行に必要なクラスのまとめ")] GameProgress gameProgress;
    [SerializeField, Tooltip("デバッグ用のログを表示するかどうか")] bool isDebug;
    [SerializeField,Tooltip("デバッグ用のテキスト表示（Setは任意）")] 


    public static GameManager instance;

    #region Unityイベント

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
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
        int index = gameProgress.user.GetIndex();
        if (index == -1) return false;
        //正常に持っているか確認
        if (gameProgress.dataSharingClass.ID[index] != 0)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// データ共有クラスのタイムリストから自身の順位を判定する
    /// 返り値で順位を戻すが、全員死亡の場合のみ0を返す
    /// 自身が死んでいる場合はDEAD
    /// </summary>
    /// <returns></returns>
    int CheckRaceRank()
    {
        var times = gameProgress.dataSharingClass.rankTime;
        float mytime = times[gameProgress.userActorNumber];
        int rank = 1;
        int deadcnt = 0;
        for (int i = 0; i < times.Count; i++)
        {
            //自身の状態をチェック
            if (i == gameProgress.userActorNumber)
            {
                if (mytime == DEAD) return (int)DEAD;
            }

            //対象の死亡をチェック
            if (times[i] == DEAD)
            {
                deadcnt++;
                continue;
            }

            //タイムを比較
            if (mytime > times[i])
            {
                rank++;
            }
        }

        //全員死亡
        if (deadcnt == times.Count-1)
        {
            rank = 0;
        }

        return rank;
    }

    /// <summary>
    /// 順位を渡すとそれに応じたスコアを計算し、返す
    /// </summary>
    /// <param name="rank"></param>
    /// <returns></returns>
    int SumScore(int rank)
    {
        //死亡時はポイントなし
        if (rank == (int)DEAD) return 0;

        //順位に応じたボーナスを加算
        int addScore = (rank == 1) ? BONUS_SCORE : 0;
        return BASE_SCORE + addScore;
    }

    List<int> ScoreCalculation()
    {
        List<int> scores = new List<int>();
        foreach(var player in PhotonNetwork.PlayerList)
        {
            scores.Add(SumScore(player.GetRankStatus()));
        }

        return scores;
    }

    bool GameEnd()
    {
        foreach(var score in gameProgress.dataSharingClass.score)
        {
            if (score >= GAME_END_SCORE) return true;
        }
        return false;
    }

    /// <summary>
    /// すべてのGame系Keyをリセットする
    /// 初期化等に行う
    /// </summary>
    void AllGameKeyReset()
    {
        PhotonNetwork.LocalPlayer.SetGameReadyStatus(false);
        PhotonNetwork.LocalPlayer.SetGameInGameStatus(false);
        PhotonNetwork.LocalPlayer.SetGameEndStatus(false);
    }

    /// <summary>
    /// GameReadyKeyをtrueにし、全員がtrueであればtrueを返す
    /// この際、GameInGameKeyはfalseに戻す
    /// </summary>
    /// <returns></returns>
    bool CheckReady()
    {
        PhotonNetwork.LocalPlayer.SetGameReadyStatus(true);
        PhotonNetwork.LocalPlayer.SetGameInGameStatus(false);

        foreach(var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetGameReadyStatus()) return false;
        }
        return true;
    }

    /// <summary>
    /// GameInGameKeyをtrueにし、全員がtrueであればtrueを返す
    /// この際、GameEndKeyはfalseに戻す
    /// </summary>
    /// <returns></returns>
    bool CheckInGame()
    {
        PhotonNetwork.LocalPlayer.SetGameInGameStatus(true);
        PhotonNetwork.LocalPlayer.SetGameEndStatus(false);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetGameInGameStatus()) return false;
        }
        return true;
    }

    /// <summary>
    /// GameEndKeyをtrueにし、全員がtrueであればtrueを返す
    /// この際、GameReadyKeyはfalseに戻す
    /// </summary>
    /// <returns></returns>
    bool CheckEnd()
    {
        PhotonNetwork.LocalPlayer.SetGameEndStatus(true);
        PhotonNetwork.LocalPlayer.SetGameReadyStatus(false);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (!player.GetGameEndStatus()) return false;
        }
        return true;
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

    /// <summary>
    /// 死亡時に呼ばれる関数
    /// 自身のタイムを死亡定数にする
    /// </summary>
    public void DeadPlayer()
    {
        gameProgress.dataSharingClass.PushGoalTime(gameProgress.userActorNumber, DEAD);
        EndFaze();
    }

    /// <summary>
    /// ゴール時に呼ばれる関数
    /// その時点でのタイムを自身のタイムに保存する
    /// </summary>
    public void GoalPlayer()
    {
        gameProgress.dataSharingClass.PushGoalTime(gameProgress.userActorNumber, (float)PhotonNetwork.Time);
        EndFaze();
    }

    /// <summary>
    /// ホストによって作成されたデータ共有クラスをセットする
    /// </summary>
    /// <param name="datasharingclass"></param>
    public void SetDataSheringClass(DataSharingClass datasharingclass)
    {
        gameProgress.dataSharingClass = datasharingclass;
    }

    /// <summary>
    /// マップマネージャーを要求する
    /// </summary>
    /// <returns></returns>
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

    void DebugInfomation()
    {

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
                coroutinename = "StateEND";
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

            //進行待機
            yield return new WaitUntil(() => CheckReady());
            DebugLog("接続確認!");

        }

        //マスターの開始を待機
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("開始まで待機");
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.S));
            //状態を送信
            PhotonNetwork.LocalPlayer.SetGameInGameStatus(true);
        }
        else
        {
            Debug.Log("開始まで待機");
            yield return new WaitUntil(() => PhotonNetwork.MasterClient.GetGameInGameStatus());
        }

        //他のプレイヤーを待機
        yield return new WaitUntil(() => CheckInGame());
        DebugLog("値の初期化開始");

        //2.各値を初期化<RESET
        {
            gameProgress = new GameProgress();
            gameState = GameStatus.READY;
            isFazeEnd = false;
            stateCoroutine = null;
            instance = this;
            gameProgress.userActorNumber = PhotonNetwork.LocalPlayer.ActorNumber - 1;

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

            DebugLog("各値の初期化完了");
        }
        //3.初期化完了・他プレイヤーを待機<WAIT
        {
            DebugLog("完了まで待機中");

            //同時にゲーム開始
            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckEnd());
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
        //進行待機
        yield return new WaitUntil(() => CheckReady());

        //生成済みの障害物を再生成
        //gameProgress.mapManager

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
            DebugLog("抽選を待機");
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID.Count != 0 ? true : false);
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID[gameProgress.dataSharingClass.ID.Count - 1] == 0);
        }

        DebugLog("抽選終了");

        //進行待機
        yield return new WaitUntil(() => CheckInGame());

       //選択クラスを生成
       gameProgress.user.GenerateMouse(0);

        //選択クラスによって終了呼び出し
        while (!isFazeEnd)
        {
            //障害物候補を表示
            List<int> list = new List<int>();

            foreach (var id in gameProgress.dataSharingClass.ID)
            {
                list.Add(id);
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

                    //アイテムのIndexをセット
                    gameProgress.user.SetIndex(random);
                }
            }

            //マウスから障害物情報が送られ、userに渡れば終了
            if (CheckUserIsHave())
            {
                //Userに障害物IDを渡す
                gameProgress.user.SetItemId(gameProgress.dataSharingClass.ID[gameProgress.user.GetIndex()]);
                //UserからIndexを受け取り、そのIndexに応じたアイテムをリストから削除
                gameProgress.dataSharingClass.ResetID(gameProgress.user.GetIndex());
                EndFaze();
            }

            yield return null;
        }

        //マウス削除
        gameProgress.user.DestroyMouse();

        //障害物をリセット


        DebugLog("選択終了待機");
        //全員の障害物選択まで待機
        while (!CheckEnd())
        {
            //障害物候補を表示
            List<int> list = new List<int>();

            foreach (var id in gameProgress.dataSharingClass.ID)
            {
                list.Add(id);
            }

            gameProgress.uiManager.PushID(list);

            yield return null;
        }

        DebugLog("障害物選択終了");
        gameState++;

        //次回の選択フェーズ開始準備
        if (PhotonNetwork.IsMasterClient) gameProgress.dataSharingClass.ResetIDList();

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
        //進行待機
        yield return new WaitUntil(() => CheckReady());
        
        gameProgress.mapManager.CreativeModeStart();

        //マウス生成
        gameProgress.user.GenerateMouse(gameProgress.user.GetItemId());


        //進行待機
        yield return new WaitUntil(() => CheckInGame());
        while (!isFazeEnd)
        {
            //設置中
            //設置されたかどうかをmapManagerから受取
            if (gameProgress.mapManager.IsInstallReference())
            {
                DebugLog("設置完了");
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

        DebugLog("設置終了待機");
        //全員の障害物選択まで待機
        yield return new WaitUntil(() => CheckEnd());

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
        //進行待機
        yield return new WaitUntil(() => CheckReady());

        //キャラの出現
        gameProgress.user.GeneratePlayer();

        DebugLog("READY演出");

        DebugLog("レーススタート");
        //進行待機
        yield return new WaitUntil(() => CheckInGame());
        //キャラの操作のロックを解除

        //プレイ中の演出
        while (!isFazeEnd)
        {
            //レース中の表示、演出

            DebugLog("レース中");

            if (Input.GetKeyDown(KeyCode.G)) GoalPlayer();
            if(Input.GetKeyDown(KeyCode.U)) DeadPlayer();

            yield return null;
        }

        //進行待機
        yield return new WaitUntil(() => CheckEnd());

        //キャラを削除
        gameProgress.user.DestroyPlayer();

        gameState++;

        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    IEnumerator StateRESULT()
    {
        DebugLog("リザルトフェーズ開始");
        //進行待機
        yield return new WaitUntil(() => CheckReady());

        //順位の計算
        int rank = CheckRaceRank();
        PhotonNetwork.LocalPlayer.SetRankStatus(rank);

        //進行待機
        yield return new WaitUntil(() => CheckInGame());


        //スコアの計算
        var scorelist = ScoreCalculation();
        gameProgress.dataSharingClass.PushScore(gameProgress.userActorNumber, scorelist[gameProgress.userActorNumber]);

        DebugLog("順位、スコアの反映演出");
        yield return new WaitForSeconds(2.0f);

        DebugLog("演出終了");
        //進行待機
        yield return new WaitUntil(() => CheckEnd());


        if (GameEnd())
        {
            DebugLog("ゲーム終了");
            gameState = GameStatus.END;
        }
        else
        {
            DebugLog("選択フェーズに返る");
            gameState = GameStatus.SELECT;
        }

        //ステートコルーチンの終了処理
        ClearCoroutine();

    }

    IEnumerator StateEND()
    {
        //終了演出

        yield return new WaitForSeconds(2.0f);

        //ネットワークから切断
        PhotonNetwork.Disconnect();

        //タイトルに移動
        SceneManager.LoadScene(SceneNames.Lobby.ToString());
    }

    #endregion

    #endregion
}
