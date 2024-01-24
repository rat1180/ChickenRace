using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PhotonMethods;
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
        BGM_TITLE,
        BGM_INGAME,

        BGM_PANIC
    }

    public enum SECode
    {
        None,  //音クリア用null
        SE_Button_Move,
        SE_Click,
        SE_RoomIn,
        SE_OpenBox,
        SE_Select,
        SE_Select_Timer,
        SE_Start,
        SE_Result,
        SE_End,
        SE_End_Effect,

        SE_Damage,
        SE_Jump,
        SE_Arrow,

        SE_Goal_Voice,
        SE_Goal,
    }
}

public class SoundManager : MonoBehaviour
{
    public GameObject SEClass;

    [SerializeField] BGMCode nowBGM;
    AudioSource audioSource;
    public static SoundManager instance;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        nowBGM = BGMCode.BGM_TITLE;
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
        var obj = SEClass.name.SafeInstantiate(new Vector3(0, 0), Quaternion.identity);
        obj.GetComponent<SEClass>().PlaySE(id);
    }

    public void SimplePlaySE(SECode id)
    {
        var obj = SEClass.name.SafeInstantiate(new Vector3(0, 0), Quaternion.identity);
        obj.GetComponent<SEClass>().SimplePlaySE(id);
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
