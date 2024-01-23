using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    enum AnimCode
    {
        None,
        Start,
        StartEnd,
        Select,
        End
    }

    enum AnimObject
    {
        StartEffect,
        SelectEffect,
        EndEffect,
        Count
    }

    List<GameObject> EffectObject = new List<GameObject>();
    public GameObject expEffect;
    public GameObject selectEffect;

    // Start is called before the first frame update
    void Start()
    {
        Init();
        
        //�e�X�g
        //StartCoroutine(SelectEffect());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Init()
    {
        for(int i = 0; i < (int)AnimObject.Count; i++)
        {
            EffectObject.Add(transform.GetChild(i).gameObject);
        }

        foreach (var obj in EffectObject) obj.SetActive(false);
    }

    /// <summary>
    /// �Q�[���X�^�[�g���̉��o
    /// </summary>
    /// <returns></returns>
    public IEnumerator StartEffect()
    {
        var obj = EffectObject[(int)AnimObject.StartEffect];
        obj.SetActive(true);
        obj.transform.GetChild(0).gameObject.SetActive(true);
        var anim = obj.GetComponent<Animator>();
        for (int i = 0; i < 3; i++)
        {
            anim.SetTrigger("Start");
            SoundManager.instance.SimplePlaySE(SoundName.SECode.SE_Click);
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("None") ? true : false);
        }

        obj.transform.GetChild(0).gameObject.SetActive(false);
        anim.SetTrigger("End");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("None") ? true : false);
        obj.SetActive(false);
    }

    public IEnumerator SelectEffect()
    {
        var obj = EffectObject[(int)AnimObject.SelectEffect];
        obj.SetActive(true);
        obj.transform.GetChild(1).GetComponent<SpriteRenderer>().color = Color.white;
        var anim = obj.GetComponent<Animator>();

        anim.SetTrigger("SelectStart");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("None") ? true : false);
        SoundManager.instance.SimplePlaySE(SoundName.SECode.SE_Select_Timer);

        anim.SetTrigger("Select");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("None") ? true : false);
        SoundManager.instance.SimplePlaySE(SoundName.SECode.SE_OpenBox);

        //�����G�t�F�N�g
        Instantiate(expEffect, obj.transform.GetChild(1).position, Quaternion.identity);

        obj.SetActive(false);
    }

    public IEnumerator EndEffect()
    {
        var obj = EffectObject[(int)AnimObject.EndEffect];
        obj.SetActive(true);

        obj.transform.GetChild(0).gameObject.SetActive(true);
        //���Đ�
        SoundManager.instance.SimplePlaySE(SoundName.SECode.SE_End_Effect);
        SoundManager.instance.SimplePlaySE(SoundName.SECode.SE_End);
        yield return new WaitForSeconds(5);
    }

    /// <summary>
    /// �I�����Ƀ}�E�X�ɉ��o
    /// </summary>
    /// <param name="pos"></param>
    public void PopSelectEffect(Vector2 pos)
    {
        Instantiate(selectEffect, pos, Quaternion.identity);
    }
}
