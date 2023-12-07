using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_ArrowShot : MonoBehaviour
{
    Rigidbody2D rb;
    float speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Moving();
    }
    //íºêiÇ≥ÇπÇÈ
    void Moving()
    {
        rb.velocity = this.transform.up * speed;
    }
    public void ArrowShot(float rot)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
    }
    public void BlowArrow(Vector3 qtn,float power)
    {
        this.transform.eulerAngles = qtn;

        speed *= power;
        Debug.Log(speed);
    }
    //void OnBecameInvisible()
    //{
    //    Destroy(this.gameObject);
    //}
}
