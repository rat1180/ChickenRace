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
    public Text text;
    bool isOn;
    // Start is called before the first frame update
    void Start()
    {
        isOn = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        NameOn();
    }

    public void NameStart()
    {

    }

    void NameOn()
    {
        if (!isOn) return;
        target = transform.parent.gameObject;

        if (target == null)
        {
            gameObject.SetActive(false);
            return;
        }
        gameObject.SetActive(true);
        var pos = text.gameObject.transform.localScale;
        if (target.transform.localScale.x < 0 && pos.x > 0) pos.x *= -1;
        if (target.transform.localScale.x > 0 && pos.x < 0) pos.x *= -1;
        text.transform.localScale = pos;
        //transform.position = Camera.main.WorldToScreenPoint(target.transform.position);
        text.text = target.GetPhotonView().Owner.NickName;
    }
}
