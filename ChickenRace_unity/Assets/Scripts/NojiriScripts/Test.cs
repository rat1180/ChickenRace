using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public Dictionary<int, Vector2Int> dic;

    // Start is called before the first frame update
    void Start()
    {
        dic.Add(0, new Vector2Int(0, 0));
        dic.Add(0, new Vector2Int(1, 0));
        dic.Add(1, new Vector2Int(2, 0));
        dic.Add(2, new Vector2Int(3, 0));
        dic.Add(2, new Vector2Int(4, 0));

        Debug.Log(dic[2]);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
