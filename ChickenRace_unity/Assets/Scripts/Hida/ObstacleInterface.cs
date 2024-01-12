using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleInterface : MonoBehaviour
{
    float gridScale;  //グリッドのサイズ。これに合わせて障害物のスケールを変更する
    float scaleZ;

    // Start is called before the first frame update
    void Start()
    {
        gridScale = transform.localScale.x;
        scaleZ = 1;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetGridScale(float size)
    {
        gridScale = size;
        ScaleRevision();
    }

    void ScaleRevision()
    {
        transform.localScale = new Vector3(gridScale, gridScale, scaleZ);
    }
}
