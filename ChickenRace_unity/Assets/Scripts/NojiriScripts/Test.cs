using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UsedGrid
{
    public List<Vector2Int> pos;
    public List<int> id;
}


public class Test : MonoBehaviour
{
    [SerializeField] private UsedGrid usedGrid;
    [SerializeField] private Dictionary<Vector2Int, int> dic;

    int aaa;
    Vector2Int bbb;
    //[SerializeField] private Dictionary<int, Vector2Int> dic = new Dictionary<int, Vector2Int>();

    // Start is called before the first frame update
    void Start()
    {
        dic = new Dictionary<Vector2Int, int>();
        foreach (Vector2Int key in dic.Keys)
        {
            for(int i = 0; i < dic.Count; i++)
            {
                usedGrid.pos[i] = key;
            }
        }

        dic.Add(new Vector2Int(0, 1), 0);
        dic.Add(new Vector2Int(1, 2), 1);
        dic.Add(new Vector2Int(2, 3), 1);
        dic.Add(new Vector2Int(3, 4), 2);
        dic.Add(new Vector2Int(4, 5), 0);

        //foreach(var item in dic)
        //{
        //    usedGrid.pos = dic.;
        //    Debug.Log("KeyF" + item.Key + ", ValueF" + item.Value);
        //}

        for(int i = 0; i < dic.Count; i++)
        {
            Debug.Log(dic.Keys);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
