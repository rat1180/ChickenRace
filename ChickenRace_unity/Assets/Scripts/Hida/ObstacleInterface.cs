using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleInterface : MonoBehaviour
{
    float gridScale;  //�O���b�h�̃T�C�Y�B����ɍ��킹�ď�Q���̃X�P�[����ύX����
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
