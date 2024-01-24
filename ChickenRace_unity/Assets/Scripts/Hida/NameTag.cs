using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class NameTag : MonoBehaviour
{
    string name;
    GameObject target;
    public float offset;
    // Start is called before the first frame update
    void Start()
    {
        NameOn(null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void NameOn(GameObject player)
    {
        if (player == null) target = null;
        else target = player;

        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        transform.position = RectTransformUtility.WorldToScreenPoint(
             Camera.main,
             target.transform.position + Vector3.up * offset);
        GetComponent<Text>().text = PhotonNetwork.LocalPlayer.NickName;
    }
}
