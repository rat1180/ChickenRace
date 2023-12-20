using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_PaunchShot : MonoBehaviour
{
    const float waitTime = 2.0f;  //巻き戻しまでの時間
    const float goSpeed  = 3.0f;  //発射スピード
    const float backSpeed = -1.0f;//巻き戻しスピード

    float speed;
    bool stopFlg;
    [SerializeField]
    Vector3 StartPos;
    [SerializeField]
    Vector3 endPos; //拳の終着点
    [SerializeField]
    Vector3 backPos;//巻き戻しの終着点

    Vector3 thisPos;
    Rigidbody2D rbb;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {       
        thisPos = this.transform.position;
        Debug.Log(thisPos);
        if (this.transform.position.x <= StartPos.x)
        {
            thisPos.x = -(thisPos.x);
        }
        if (this.transform.position.y <= StartPos.y)
        {
            thisPos.y = -(thisPos.y);
        }

        if (thisPos.x < endPos.x && thisPos.y < endPos.y)
        {
            if (!stopFlg)
            {
                Paunching();
            }
        }
        else
        {
            stopFlg = true;
            rbb.velocity = Vector3.zero;
            StartCoroutine(DelayBack(waitTime));
        }
    }
    //直進させる
    protected virtual void Paunching()
    {
        speed = goSpeed;
        rbb.velocity = this.transform.up * speed;   
    }
    /// <summary>
    /// 角度取得
    /// </summary>
    /// <param name="rot"></param>
    public void PaunchShot(float rot)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
    }
    /// <summary>
    /// 巻き戻しの遅延
    /// </summary>
    /// <param name="wt"></param>
    /// <returns></returns>
    IEnumerator DelayBack(float wt)
    {
        speed = backSpeed;
        yield return new WaitForSeconds(wt);
        rbb.velocity = this.transform.up * speed;

        if (thisPos.x < backPos.x || thisPos.y < backPos.y)
        {
            Debug.Log("拳");
            rbb.velocity = Vector3.zero;
            Destroy(this.gameObject);
        }
    }
    void Init()
    {
        rbb = this.transform.GetComponent<Rigidbody2D>();
        stopFlg = false;
        StartPos = this.transform.position;

        // マイナス方向に進む場合めっちゃ進んじゃう、おかしくなる
        endPos = this.transform.position;
        endPos.x += 1.0f;
        endPos.y += 1.0f;

        backPos = this.transform.position;

        backPos.x -= 1.0f;
        backPos.y -= 1.0f;
    }
    //void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
}
