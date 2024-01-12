using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenerateObstacle : MonoBehaviourPun
{
    [SerializeField] GameObject obstacle;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetObstacleID(int id, float angle, Vector2Int gridPos)
    {
        //idÇ©ÇÁè·äQï®Çåüçı
        //obstacleName = 

        int x, y;
        x = gridPos.x;
        y = gridPos.y;
        photonView.RPC("Generate", RpcTarget.Others,id,angle,x,y);

        Destroy(gameObject);
    }

    [PunRPC]
    void Generate(int id, float angle,int x,int y)
    {
        Vector2Int vector2int = new Vector2Int(x, y);
        GameManager.instance.GetMapManager().GetComponent<MapManager>().SpawnObstacle(id, angle, vector2int);
    }


    //çÌèúóp
    public void DeleteObstacle(Vector2Int gridPos)
    {
        int x, y;
        x = gridPos.x;
        y = gridPos.y;
        photonView.RPC("Delete", RpcTarget.Others,x, y);

        Destroy(gameObject);
    }

    [PunRPC]
    void Delete(int x,int y)
    {
        Vector2Int vector2int = new Vector2Int(x, y);
        GameManager.instance.GetMapManager().GetComponent<MapManager>().RemoveObstacle(vector2int);
    }

}