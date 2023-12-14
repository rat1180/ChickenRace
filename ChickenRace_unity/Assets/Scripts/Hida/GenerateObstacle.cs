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

    public void SetObstacleID(int id)
    {
        //idÇ©ÇÁè·äQï®Çåüçı
        //obstacleName = 

        photonView.RPC("Generate", RpcTarget.All);

        Destroy(gameObject);
    }

    [PunRPC]
    void Generate()
    {
        Instantiate(obstacle, transform.position, Quaternion.identity);
    }
}