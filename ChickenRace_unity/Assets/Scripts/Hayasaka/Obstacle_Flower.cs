using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_Flower : Obstacle
{
    [SerializeField]
    bool isFlowerLeftPaunchFlg;
    [SerializeField]
    bool isFlowerRightPaunchFlg;
    [SerializeField]
    bool isFlowerPaunchFlg;
    [SerializeField]
    bool isFlowerPaunchWaitFlg;
    [SerializeField]
    float speed;

    [SerializeField]
    GameObject paunch;
    [SerializeField]
    GameObject paunchTargetLeft;
    [SerializeField]
    GameObject paunchTargetRight;

    [SerializeField]
    Vector2 thisPos;
    [SerializeField]
    Vector2 PaunchPos;
    [SerializeField]
    Rigidbody2D rb;
    /// <summary>
    /// 初期化
    /// </summary>
    protected override void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);

        PaunchPos = paunch.transform.position;

        rb = paunch.transform.GetComponent<Rigidbody2D>();

        isFlowerLeftPaunchFlg = false;
        isFlowerRightPaunchFlg = false;
        isFlowerPaunchWaitFlg = false;
        isFlowerPaunchFlg = false;

        speed = 5.0f;
    }
    protected override void update()
    {
        thisPos = this.gameObject.transform.position;
        //if (GameManager.instance.CheckObstacleMove())
        {
            if (isFlowerLeftPaunchFlg)
            {
                LeftShot();
                isFlowerPaunchWaitFlg = true;
            }
            if(isFlowerRightPaunchFlg)
            {
                RightShot();
                isFlowerPaunchWaitFlg = true;
            }
        }
    }
    protected override void ObjStart()
    {
        
    }
    void RightShot()
    {
        var newTrans = PaunchPos;
        newTrans.x += 2.0f;
        if (paunch.transform.position.x < newTrans.x && !isFlowerPaunchFlg)
        {
            Debug.Log("右");
            rb.velocity = paunch.transform.right * speed;
        }
        else
        {
            rb.velocity = new Vector2(0, 0);
            isFlowerPaunchFlg = true;
            Invoke("WaitFlg", 4.0f);
            Debug.Log("戻り");
            paunch.transform.position = Vector3.MoveTowards(paunch.transform.position, thisPos, speed * Time.deltaTime);
        }
        Debug.Log(newTrans);
    }
    void LeftShot()
    {
        var newTrans = PaunchPos;
        if (isFlowerLeftPaunchFlg && !isFlowerRightPaunchFlg)
        {
            newTrans.x -= 2.0f;
            if (paunch.transform.position.x >= newTrans.x)
            {
                Debug.Log("左");
                rb.velocity = paunch.transform.right * -speed;
            }
            else
            {
                paunch.transform.position = Vector3.MoveTowards(paunch.transform.position, thisPos, speed * Time.deltaTime);
            }
        }
        if (isFlowerRightPaunchFlg && !isFlowerLeftPaunchFlg)
        {
            newTrans.x += 2.0f;
            if (paunch.transform.position.x <= newTrans.x && !isFlowerPaunchFlg)
            {
                Debug.Log("右");
                rb.velocity = paunch.transform.right * speed;
            }
            else
            {
                isFlowerPaunchFlg = true;
                Debug.Log("戻り");
                paunch.transform.position = Vector3.MoveTowards(paunch.transform.position, thisPos, speed * Time.deltaTime);
            }
            Debug.Log(newTrans);
        }
        Invoke("WaitFlg", 4.0f);
    }
    void WaitFlg()
    {
        isFlowerPaunchWaitFlg = false;
        isFlowerPaunchFlg = false;
        isFlowerLeftPaunchFlg = false;
        isFlowerRightPaunchFlg = false;
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var toucheObj = other.gameObject.transform.position;
        //パンチが戻るまで判定しない
        if (!isFlowerPaunchWaitFlg)
        {
            if (thisPos.x > toucheObj.x)
            {
                isFlowerLeftPaunchFlg = true;
            }
            else
            {
                isFlowerRightPaunchFlg = true;
            }
        }
    }
    
}
