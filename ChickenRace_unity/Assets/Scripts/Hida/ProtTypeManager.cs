using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtTypeManager : MonoBehaviour
{
    enum GameStatus
    {
        READY,
        SELECT,
        PLANT,
        RACE,
        RESULT
    }

    //初期化の段階を示す
    //その段階が終わると次の状態に進む
    enum InitStatus
    {
        CONECT,
        RESET,
        WAIT,
        START
    }

    [SerializeField, Tooltip("現在のゲーム状態")] GameStatus gameState;


    public static ProtTypeManager Instance;

    #region Unityイベント

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region 関数

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

    #endregion

    #region コルーチン

    IEnumerator GameInit()
    {
        //状態を送信
        InitStatus initStatus = InitStatus.CONECT;

        //1.接続を確認<CONECT
        {
            //フォトンの機能で接続しているか確認
            while (false)
            {
                //接続まで待機
                Debug.Log("接続確認中..");
                yield return null;
            }

            //状態を送信
            initStatus = InitStatus.RESET;
            //他のプレイヤーを待機
            yield return new WaitUntil(() => CheckInitState(InitStatus.RESET));
            Debug.Log("接続確認!");

        }
        //2.各値を初期化<RESET
        {
            gameState = GameStatus.READY;

            //各マネージャーを生成

            //各マネージャー初期化確認・待機

            //Playerクラス生成

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
        Debug.Log("初期化完了");
    }

    #endregion
}
