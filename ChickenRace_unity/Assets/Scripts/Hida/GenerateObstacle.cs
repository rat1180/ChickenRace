using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GenerateObstacle : MonoBehaviourPun
{
    [SerializeField] string obstacleName;

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
        //id�����Q��������
        //obstacleName = 

        photonView.RPC("Generate", RpcTarget.All);

        Destroy(gameObject);
    }

    [PunRPC]
    void Generate()
    {
        PhotonNetwork.Instantiate(obstacleName, transform.position, Quaternion.identity);
    }
}
