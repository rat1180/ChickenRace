using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectImage : MonoBehaviour
{
    public int itemId;
    int index;
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
    /// <summary>
    /// Idの取得.
    /// </summary>
    /// <returns></returns>
    public int GetItemId()
    {
        return itemId;
    }

    /// <summary>
    /// Indexの取得.
    /// </summary>
    /// <returns></returns>
    public int GetIndex()
    {
        return index;
    }
}
