using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    //public Vector2 movePos;    // �ړ����������W�@�e�X�g�p

    [SerializeField] List<GameObject> GenerateList = new List<GameObject>(); // ����������Q�����X�g
    [SerializeField] List<Transform> UsedGridList = new List<Transform>(); // �ݒu������Q�����X�g

    [SerializeField] private GameObject gameObject; // �ړ��������I�u�W�F�N�g�̏��擾

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    
    //void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.tag == "UsedGrid")
    //    {
    //        Debug.Log("�u���܂���");
    //    }
    //}

    /// <summary>
    /// �O���b�h�ړ����\�b�h
    /// </summary>
    private IEnumerator StartMove()
    {
        while (true)
        {
            // �ړ����\�b�h
        }
    }

    #region �O���p���\�b�h
    /// <summary>
    /// �R���[�`���J�n�p���\�b�h
    /// ��Q���̈ړ��̍ۂɌĂяo��
    /// </summary>
    public void MapCoroutineStart()
    {
        StartCoroutine(StartMove());
    }

    /// <summary>
    /// �R���[�`���I���p���\�b�h
    /// </summary>
    public void MapCoroutineEnd()
    {
        StopCoroutine(StartMove());
    }

    /// <summary>
    /// ��Q���ݒu�����\�b�h
    /// </summary>
    public void MapMoveEnd()
    {
        // ���̈ʒu�ɌŒ�
    }
    #endregion
}
