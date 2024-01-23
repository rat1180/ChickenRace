using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SEClass : MonoBehaviourPun
{
    SoundName.SECode seCode;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySE(SoundName.SECode secode)
    {
        photonView.RPC("PlaySERPC", RpcTarget.All, (int)secode);
    }

    public void SimplePlaySE(SoundName.SECode secode)
    {
        seCode = (SoundName.SECode)secode;
        StartCoroutine(SECoroutine());
    }

    [PunRPC]
    void PlaySERPC(int secode)
    {
        seCode = (SoundName.SECode)secode;
        StartCoroutine(SECoroutine());
    }

    IEnumerator SECoroutine()
    {
        var audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(ResourceManager.instance.GetSE(seCode));
        yield return new WaitWhile(() => audioSource.isPlaying);
        Destroy(gameObject);
    }
}
