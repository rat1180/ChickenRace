using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_PaunchShot : MonoBehaviour
{
    const float waitTime = 2.0f;  //巻き戻しまでの時間
    const float backSpeed = 2.0f;//巻き戻しスピード

    float speed;
    bool stopFlg;
   
    Rigidbody2D rbb;

    GameObject Pt;
    GameObject Bt;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.x != Pt.transform.position.x && this.transform.position.y != Pt.transform.position.y)
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
        this.transform.position = Vector3.MoveTowards(transform.position, Pt.transform.position, speed * Time.deltaTime);
    }
    /// <summary>
    /// 角度とか取得
    /// </summary>
    /// <param name="rot"></param>
    public void PaunchShot(float rot,GameObject pt,GameObject bt)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
        Pt = pt;
        Bt = bt;
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
        this.transform.position = Vector3.MoveTowards(transform.position, Bt.transform.position, speed * Time.deltaTime);

        if (this.transform.position.x == Bt.transform.position.x || this.transform.position.y == Bt.transform.position.y)
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
        speed = 3.0f;
    }
    //void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
}
