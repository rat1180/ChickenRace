using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Paunch : Obstacle
{
    [SerializeField]
    bool isPaunchFlg;
    [SerializeField]
    bool isPaunchWaitFlg;

    [SerializeField]
    GameObject paunchShot;
    [SerializeField]
    GameObject paunchChild;
    [SerializeField]
    GameObject paunchTarget;
    [SerializeField]
    GameObject backTarget;
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        isPaunchFlg = false;
        isPaunchWaitFlg = false;
    }
    protected override void update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            if (isPaunchFlg)
            {
                ShotObj();
            }
        }
        if (GameManager.instance.CheckRaceEnd())
        {
            Destoroy();
        }
    }
    protected override void ObjStart()
    {
        isPaunchFlg = true;
    }
    /// <summary>
    /// 角度取得から拳生成
    /// </summary>
    void ShotObj()
    {
        Debug.Log("発射");

        myRotation = this.transform.localEulerAngles.z;
        var ps = Instantiate(paunchShot,paunchChild.transform.position, Quaternion.identity);
        ps.transform.localScale += this.transform.localScale;
        ps.GetComponent<Obstacle_PaunchShot>().PaunchShot(myRotation,paunchTarget,backTarget);
        isPaunchWaitFlg = true;
        isPaunchFlg = false;
        Invoke("WaitFlg", 6.0f);
    }
    void WaitFlg()
    {
        isPaunchWaitFlg = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        //パンチが戻るまで判定しない
        if (!isPaunchWaitFlg)
        {
            Debug.Log("パンチ");
            isPaunchFlg = true;
        }
    }
    void Destoroy()
    {
        Destroy(this.gameObject);
    }
}
