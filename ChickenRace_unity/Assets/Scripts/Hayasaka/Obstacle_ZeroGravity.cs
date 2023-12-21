using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_ZeroGravity : MonoBehaviour
{
    bool colFlg;
    void Start()
    {
        colFlg = false;
    }
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        var orb2 = other.gameObject.GetComponent<Rigidbody2D>();
        orb2.gravityScale = -1;
        colFlg = true;
        StartCoroutine(NoneGravity(1.0f,orb2));
        
    }
    //void OnTriggerExit2D(Collider2D other)
    //{
    //    other.gameObject.GetComponent<Rigidbody2D>().gravityScale = 0;
    //}
    IEnumerator NoneGravity(float wt,Rigidbody2D rb2)
    {
        yield return new WaitForSeconds(wt);
        //for (int i = 0; i < 50; i++)
        //{
        //    r2b.gravityScale += 0.02f;
        //    Debug.Log("ˆ¢•”Š°");
        //}
        //Debug.Log("drive");
        rb2.gravityScale = 0.0f;
        rb2.constraints = RigidbodyConstraints2D.FreezePositionX;
        rb2.constraints = RigidbodyConstraints2D.FreezePositionY;
        yield break;
    }
}
