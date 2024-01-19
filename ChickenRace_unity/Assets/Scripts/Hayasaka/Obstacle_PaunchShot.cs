using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_PaunchShot : MonoBehaviour
{
    const float waitTime = 2.0f;  //巻き戻しまでの時間
    const float backSpeed = 2.0f;//巻き戻しスピード
    [SerializeField]
    float speed; //通常スピード
    [SerializeField]
    bool isStopFlg;
    [SerializeField]
    Rigidbody2D rb;
    [SerializeField]
    GameObject pt;
    [SerializeField]
    GameObject bt;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            if (this.transform.position.x != pt.transform.position.x || this.transform.position.y != pt.transform.position.y)
            {
                Debug.Log("asass");
                if (!isStopFlg)
                {
                    Paunching();
                }
            }
            else
            {
                isStopFlg = true;
                rb.velocity = Vector3.zero;
                StartCoroutine(DelayBack(waitTime));
            }
        }
        if (GameManager.instance.CheckRaceEnd())
        {
            Destoroy();
        }
    }
    //直進させる
    protected virtual void Paunching()
    {
        this.transform.position = Vector3.MoveTowards(transform.position, pt.transform.position, speed * Time.deltaTime);
    }
    /// <summary>
    /// 角度とか取得
    /// </summary>
    /// <param name="rot"></param>
    public void PaunchShot(float rot,GameObject paunch_t,GameObject back_t)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
        pt = paunch_t;
        bt = back_t;
    }
    /// <summary>
    /// 巻き戻しの遅延
    /// </summary>
    /// <param name="wt"></param>
    /// <returns></returns>
    IEnumerator DelayBack(float wt)
    {
        //巻き戻す
        speed = backSpeed;
        yield return new WaitForSeconds(wt);
        this.transform.position = Vector3.MoveTowards(transform.position, bt.transform.position, speed * Time.deltaTime);
        //生成位置まで来たら消滅
        if (this.transform.position.x == bt.transform.position.x && this.transform.position.y == bt.transform.position.y)
        {
            Debug.Log("拳");
            rb.velocity = Vector3.zero;
            Destroy(this.gameObject);
        }
    }
    void Init()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        isStopFlg = false;
        speed = 3.0f;
    }
    //void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
    void Destoroy()
    {
        Destroy(this.gameObject);
    }
}
