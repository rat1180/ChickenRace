using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundName;

namespace SoundName
{
    //リソース探索用に全ての音素材のまとめ
    public enum SoundCode
    {
        None,  //音クリア用null
        BGM_Title,
        BGM_Mode,
        BGM_Wait,
        BGM_Ready,
        BGM_Race,
        BGM_END,
        SE_Click,
        SE_RoomIn,
        SE_OpenBox,
        SE_Select,
        SE_Start,
        SE_Result,
    }

    public enum BGMCode
    {
        None,  //音クリア用null
        BGM_Title,
        BGM_Mode,
        BGM_Wait,
        BGM_Ready,
        BGM_Race,
        BGM_END,

        BGM_PANIC
    }

    public enum SECode
    {
        None,  //音クリア用null
        SE_Click,
        SE_RoomIn,
        SE_OpenBox,
        SE_Select,
        SE_Start,
        SE_Result,
    }
}

public class SoundManager : MonoBehaviour
{
    List<AudioClip> SoundList = new List<AudioClip>();

    [SerializeField] BGMCode nowBGM;
    AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        nowBGM = BGMCode.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayBGM(nowBGM);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            StopBGM();
        }
    }

    public void PlaySE(SECode id)
    {
        audioSource.PlayOneShot(ResourceManager.instance.GetSE(id));
    }

    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void PlayBGM(BGMCode id)
    {
        //audioSource.clip = BGMList[(int)id];
        audioSource.clip = ResourceManager.instance.GetBGM(id);
        audioSource.Play();
    }
}
