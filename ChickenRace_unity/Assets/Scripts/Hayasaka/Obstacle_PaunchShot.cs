using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_PaunchShot : MonoBehaviour
{
    const float waitTime = 2.0f;
    const float goSpeed  = 3.0f;
    const float backSpeed = -1.0f;
    Rigidbody2D rbb;
    float speed;
    bool stopFlg;
    [SerializeField]
    Vector3 endPos;
    [SerializeField]
    Vector3 backPos;
    // Start is called before the first frame update
    void Start()
    {
        rbb = this.transform.GetComponent<Rigidbody2D>();
        stopFlg = false;
        endPos = this.transform.position;
        endPos.y += 1;

        backPos = this.transform.position;
        backPos.y -= 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y < endPos.y)
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
    //’¼i‚³‚¹‚é
    protected virtual void Paunching()
    {
        speed = goSpeed;
        rbb.velocity = this.transform.up * speed;   
    }
    public void PaunchShot(float rot)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
    }
    IEnumerator DelayBack(float wt)
    {
        speed = backSpeed;
        yield return new WaitForSeconds(wt);
        rbb.velocity = this.transform.up * speed;

        if (this.transform.position.y < backPos.y)
        {
            rbb.velocity = Vector3.zero;
            Destroy(this.gameObject);
        }
    }
    //void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
}
