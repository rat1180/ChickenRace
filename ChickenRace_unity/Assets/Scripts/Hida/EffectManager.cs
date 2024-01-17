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
    }

    enum AnimObject
    {
        StartEffect,
        //Select,
        Count
    }

    List<GameObject> EffectObject = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Init();
        
        //テスト
        //StartCoroutine(StartEffect());
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
    /// ゲームスタート時の演出
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
            yield return new WaitForSeconds(0.5f);
            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("None") ? true : false);
        }

        obj.transform.GetChild(0).gameObject.SetActive(false);
        anim.SetTrigger("End");
        yield return new WaitForSeconds(0.5f);
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("None") ? true : false);
    }
}
