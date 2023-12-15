using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ResorceNames;

public class TestCreate : MonoBehaviour
{
    public OBSTACLE_OBJECT objectName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            Instantiate(ResorceManager.instance.GetObstacleObject(objectName), transform.position, Quaternion.identity);
        }
    }
}
