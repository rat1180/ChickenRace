using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Dictionary;

public class Test : MonoBehaviour
{
    [SerializeField] Dictionary_Unity<int, string> test;
    // Start is called before the first frame update
    void Start()
    {
        test = new Dictionary_Unity<int, string>();
        test.Add(1, "a");
        if (test.ContainKey(1)) Debug.Log("ok");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
