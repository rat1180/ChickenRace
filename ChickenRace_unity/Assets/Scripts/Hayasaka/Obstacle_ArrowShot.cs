using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_ArrowShot : MonoBehaviour
{
    [SerializeField]
    protected Rigidbody2D rb;
    [SerializeField]
    protected float speed;
    // Start is called before the first frame update
    void Start()
    {
        rb = this.transform.GetComponent<Rigidbody2D>();
        speed = 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            Moving();
        }
        if (GameManager.instance.CheckRaceEnd())
        {
            Destoroy();
        }
    }
    //直進させる
    protected virtual void Moving()
    {
        rb.velocity = this.transform.up * speed;
    }
    public void ArrowShot(float rot)
    {
        this.transform.rotation = Quaternion.Euler(0, 0, rot);
    }
    public void BlowArrow(Quaternion qtn,float power)
    {
        StartCoroutine(DelayBA(qtn, power,0.25f));
    }
    IEnumerator DelayBA(Quaternion qtn, float power,float dt)
    {
        yield return new WaitForSeconds(dt);
        this.transform.rotation = qtn;

        speed *= power;
        Debug.Log(speed);
        
    }
    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
    void Destoroy()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destoroy();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destoroy();
    }
}
