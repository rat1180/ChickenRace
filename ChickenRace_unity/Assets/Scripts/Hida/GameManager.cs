using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    enum GameStatus
    {
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

    /// <summary>
    /// ゲームの進行に必要なマネージャー等をまとめたクラス
    /// </summary>
    [System.Serializable]
    public class GameProgress
    {
        public MapManager mapManager;
        public Image uiManager;
        public DataSharingClass dataSharingClass;
    }

    [SerializeField, Tooltip("現在のゲーム状態")] GameStatus gameState;
    [SerializeField, Tooltip("進行中のステートコルーチン")] Coroutine stateCoroutine;
    [SerializeField, Tooltip("他のクラスから渡される、現在のフェーズ終了を知らせる変数")] bool isFazeEnd; //Int型にして複数の状態に対応出来るようにするかも
    [SerializeField, Tooltip("ゲームの進行に必要なクラスのまとめ")] GameProgress gameProgress;
    [SerializeField, Tooltip("デバッグ用のログを表示するかどうか")] bool isDebug;


    public static GameManager Instance;

    #region Unityイベント

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameInit());
    }

    // Update is called once per frame
    void Update()
    {
        GameLoop();

        if(Input.GetKeyDown(KeyCode.Space)){
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
        InitStatus playerinitlist = status;
        if (playerinitlist == status) return true;
        else return false;
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

    public void SetDataSheringClass(DataSharingClass datasharingclass)
    {
        gameProgress.dataSharingClass = datasharingclass;
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
                break;
            case GameStatus.RESULT:
                break;
            case GameStatus.END:
                break;
        }
        if(coroutinename == "null")
        {
            DebugLogWarning("コルーチンが正常に振り分けられていません");
            return;
        }
        stateCoroutine = StartCoroutine(coroutinename);
    }

    #endregion

    #region コルーチン

    IEnumerator GameInit()
    {
        //状態を送信
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
            initStatus = InitStatus.RESET;
            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckInitState(InitStatus.RESET));
            DebugLog("接続確認!");

        }
        //2.各値を初期化<RESET
        {
            gameState = GameStatus.READY;
            isFazeEnd = false;
            stateCoroutine = null;
            Instance = this;

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

            //状態を送信
            initStatus = InitStatus.WAIT;
            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckInitState(InitStatus.WAIT));
        }
        //3.初期化完了・他プレイヤーを待機<WAIT
        {
            //同時にゲーム開始
            //状態を送信
            initStatus = InitStatus.START;
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
            for(int i = 0; i < 4; i++)
            {
                int id = Random.Range(1, 4);
                //障害物追加
                gameProgress.dataSharingClass.PushID(i == 3 ? 0 : id);
            }
        }
        //ゲストなら抽選まで待機
        else
        {
            yield return new WaitUntil(() => gameProgress.dataSharingClass.ID[gameProgress.dataSharingClass.ID.Count-1] == 0);
        }

        //選択クラスを生成

        //選択クラスによって終了呼び出し
        while (!isFazeEnd)
        {
            //障害物候補を表示
            var list = gameProgress.dataSharingClass.ID;

            //時間制限にかかれば終了

            //障害物選択まで待機
            yield return null;
        }

        DebugLog("障害物選択終了");

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
        gameProgress.mapManager.CreativeModeStart();

        while (!isFazeEnd)
        {
            //設置中
            //設置されたかどうかをmapManagerから受取
            if (true)
            {

            }

            //全員が設置完了するか時間切れで設置完了
            if (true)
            {

            }

            yield return null;
        }

        DebugLog("障害物設置終了");

        //ステートコルーチンの終了処理
        ClearCoroutine();
    }

    #endregion

    #endregion
}
