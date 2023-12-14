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
    /// <summary>
    /// èâä˙âª
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
        isPaunchFlg = false;
        isPaunchWaitFlg = false;
    }
    protected override void update()
    {
        if (isPaunchFlg)
        {       
           ShotObj();
        }
       
    }
    protected override void ObjStart()
    {
        isPaunchFlg = true;
    }
    void ShotObj()
    {
        Debug.Log("î≠éÀ");

        myRotation = this.transform.localEulerAngles.z;
        var ps = Instantiate(paunchShot,paunchChild.transform.position, Quaternion.identity);
        ps.GetComponent<Obstacle_PaunchShot>().PaunchShot(myRotation);
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
        if (!isPaunchWaitFlg)
        {
            Debug.Log("ÉpÉìÉ`");
            isPaunchFlg = true;
        }
    }
    //void Destoroy()
    //{
    //    Destroy(this.gameObject);
    //}
}
