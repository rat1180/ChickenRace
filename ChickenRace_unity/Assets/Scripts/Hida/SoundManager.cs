using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SoundName;

namespace SoundName
{
    //���\�[�X�T���p�ɑS�Ẳ��f�ނ̂܂Ƃ�
    public enum SoundCode
    {
        None,  //���N���A�pnull
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
        None,  //���N���A�pnull
        BGM_Title,
        BGM_Mode,
        BGM_Wait,
        BGM_Ready,
        BGM_Race,
        BGM_END,
    }

    public enum SECode
    {
        None,  //���N���A�pnull
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
    [SerializeField] AudioSource audioSource;

    private GameObject seObject;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        nowBGM = BGMCode.None;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySE(SECode se_code)
    {

    }
}
