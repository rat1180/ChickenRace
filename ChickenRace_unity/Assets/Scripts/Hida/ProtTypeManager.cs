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

    enum InitStatus
    {
        READY,
        NOW,
        WAIT
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
    /// 全員がWait状態の時にtrueを返す
    /// この初期化状態はカスタムプロパティで管理する
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    bool CheckInitState()
    {
        //全員の初期化状態を確認
        InitStatus playerinitlist = InitStatus.WAIT;
        if (playerinitlist == InitStatus.WAIT) return true;
        else return false;

    }

    #endregion

    #region コルーチン

    IEnumerator GameInit()
    {
        //状態を送信
        InitStatus initStatus = InitStatus.READY;

        //1.接続を確認
        //状態を送信
        initStatus = InitStatus.NOW;
        //フォトンの機能で接続しているか確認
        while (false)
        {
            //接続まで待機
            Debug.Log("接続確認中..");
            yield return null;
        }

        //状態を送信
        initStatus = InitStatus.WAIT;
        //他のプレイヤーを待機
        yield return new WaitUntil(() => CheckInitState());

        //2.各値を初期化
        gameState = GameStatus.READY;

        //各マネージャーを生成

        //各マネージャー初期化確認・待機

        //Playerクラス生成

        //初期化完了・他プレイヤーを待機
    }

    #endregion
}
