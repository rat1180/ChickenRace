using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Arrow : Obstacle
{
    [SerializeField]
    float shotCnt;

    [SerializeField]
    bool isShotFlg;

    [SerializeField]
    GameObject arrowShot;
    [SerializeField]
    GameObject arrowChild;
    /// <summary>
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        shotCnt = 3.0f;
        isShotFlg = true;
    }
    protected override void update()
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
    protected override void ObjStart()
    {
        shotCnt = 3.0f;
        isShotFlg = true;
    }
    void ShotObj()
    {
        Debug.Log("î≠éÀ");

        myRotation = this.transform.localEulerAngles.z;
        var ars = Instantiate(arrowShot,arrowChild.transform.position, Quaternion.identity);
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
