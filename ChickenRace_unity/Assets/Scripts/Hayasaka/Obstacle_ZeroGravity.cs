using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle_ZeroGravity : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        var orb2 = other.gameObject.GetComponent<Rigidbody2D>();
        orb2.gravityScale = orb2.gravityScale / 5;       
        //StartCoroutine(NoneGravity(1.0f,orb2));        
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var orb2 = other.gameObject.GetComponent<Rigidbody2D>();
        orb2.gravityScale = orb2.gravityScale * 5;
        //StartCoroutine(NoneGravity(1.0f, orb2));
    }
    /// <summary>
    /// èdóÕílÇÃêÿÇËë÷Ç¶
    /// </summary>
    /// <param name="wt"></param>
    /// <param name="rb2"></param>
    /// <returns></returns>
    IEnumerator NoneGravity(float wt,Rigidbody2D rb2)
    {
        yield return new WaitForSeconds(wt);
        rb2.gravityScale = 1.0f;
        yield return new WaitForSeconds(1.0f);
        rb2.gravityScale = 0.0f;
        rb2.constraints = RigidbodyConstraints2D.FreezePosition;
        rb2.constraints = RigidbodyConstraints2D.None;
        yield break;
    }
}
