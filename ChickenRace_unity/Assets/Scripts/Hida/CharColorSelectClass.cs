using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CharColorSelectClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var image = transform.Find("Body").gameObject;
        image.GetComponent<SpriteRenderer>().sprite = ResourceManager.instance.GetCharcterImage(GetComponent<Character>().photonView.OwnerActorNr);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
