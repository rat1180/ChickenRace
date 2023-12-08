using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_ParabolaArrowShot : Obstacle_ArrowShot
{
    float hheight;
    Vector3 eend;
    Vector3 sstart;
    // Start is called before the first frame update
    void Start()
    {
        hheight = 3.0f;
        sstart = this.transform.position;
        eend = sstart;
        eend.x += 5.0f;
        eend.y += 5.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Moving(hheight, sstart, eend);
    }
    protected void Moving(float height, Vector3 start, Vector3 end)
    {
        Vector3 half = end - start * 0.50f + start;
        half.y += Vector3.up.y + height;

        StartCoroutine(LerpThrow(this.gameObject, start, half, end, speed));
    }
    IEnumerator LerpThrow(GameObject target, Vector3 start, Vector3 half, Vector3 end, float duration)
    {
        float startTime = Time.timeSinceLevelLoad;
        float rate = 0f;
        while (true)
        {
            if (rate >= 1.0f)
                yield break;

            float diff = Time.timeSinceLevelLoad - startTime;
            rate = diff / (duration / 60f);
            target.transform.position = CalcLerpPoint(start, half, end, rate);

            yield return null;
        }
    }
    Vector3 CalcLerpPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        var a = Vector3.Lerp(p0, p1, t);
        var b = Vector3.Lerp(p1, p2, t);
        return Vector3.Lerp(a, b, t);
    }
}
