using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PhotonMethods;
public class Obstacle_Arrow : Obstacle
{
    [SerializeField]
    float shotCnt; //発射間隔
    [SerializeField]
    bool isShotFlg;
    [SerializeField]
    GameObject arrowShot;
    [SerializeField]
    GameObject arrowChild;
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        shotCnt = 3.0f;
        isShotFlg = true;
    }
    protected override void update()
    {
        //if (GameManager.instance.CheckObstacleMove())
        {
            if (isShotFlg)
            {
                if (shotCnt > 0.0f)
                {
                    shotCnt -= Time.deltaTime;
                }
                if (shotCnt < 0.0f)
                {
                    ShotObj();
                    shotCnt = 3.0f;
                }
            }
        }
    }
    protected override void ObjStart()
    {
        shotCnt = 3.0f;
        isShotFlg = true;
    }
    /// <summary>
    /// 自身の角度を取得し、矢を生成し、発射関数を呼び出す。
    /// </summary>
    void ShotObj()
    {
        //Debug.Log("発射");

        myRotation = this.transform.localEulerAngles.z;
        var ars = "Obstacle/Damage_ArrowShot".SafeInstantiate(arrowChild.transform.position, Quaternion.identity);
        ars.GetComponent<Obstacle_ArrowShot>().ArrowShot(myRotation);
    }
    void OnTrggerEnter2D(Collision other)
    {
        Destoroy();
    }
    void Destoroy()
    {
        Destroy(this.gameObject);
    }
}
