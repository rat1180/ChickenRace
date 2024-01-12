using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    //リソース探索用に全ての音素材のまとめ
    enum SoundCode
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

    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
