using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;

public class UIManager : MonoBehaviour
{
    private GameObject imageObjects;
    private GameObject resultPanel;
    private GameObject resultCharacters;//���U���g��ʂ̃L�����N�^�[(Name,Sciore,Rank�\��)
    private List<int> id;
    //[SerializeField, Range(0, 3)] int testSoldOutInex;

    public string[] names = new string[4];

    private void Start()
    {
        imageObjects = gameObject.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
        resultPanel = gameObject.transform.GetChild(0).gameObject.transform.GetChild(2).gameObject;
        resultCharacters = resultPanel.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            PushNameTest();
        }
        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    SoldOut(testSoldOutInex);
        //}
    }

    /// <summary>
    /// �S������Q���̑I�����I��������UI���\���ɂ���.
    /// </summary>
    public void FinishSelect()
    {
        imageObjects.SetActive(false);
    }

    /// <summary>
    /// GameManager����ID������֐�
    /// </summary>
    public void PushID(List<int> iD)
    {
        id = new List<int>();
        id = iD;
        imageObjects.SetActive(true);
        ChangeObstacleImage();
    }

    /// <summary>
    /// 4�̉摜��ς���
    /// ������ID���t�^����
    /// </summary>
    public void ChangeObstacleImage()
    {
        for(int i = 0; i < imageObjects.transform.childCount; i++)
        {
            //imageObjects.transform.GetChild(i).GetComponent<Image>().sprite =
            imageObjects.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite =
            ResourceManager.instance.GetObstacleImage(id[i]);//�摜��ύX.
            //�R���|�[�l���g�����݂��Ȃ���Βǉ�����ID�������A�t�ɑ��݂���΂��̂܂�ID��������
            if (imageObjects.transform.GetChild(i).gameObject.GetComponent<ObstacleImage>() == null)
            {
                imageObjects.transform.GetChild(i).gameObject.AddComponent<ObstacleImage>().id = i;
            }
            else
            {
                imageObjects.transform.GetChild(i).gameObject.GetComponent<ObstacleImage>().id = i;
            }
        }
    }

    /// <summary>
    /// index���w�肵�Ċ������(0)�ɂ���
    /// </summary>
    public void SoldOut(int index)
    {
        imageObjects.transform.GetChild(index).GetComponent<Image>().sprite =
            ResourceManager.instance.GetObstacleImage(OBSTACLE_IMAGE_NAMES.Kanbaipop);//�����摜�ɕύX.
        imageObjects.transform.GetChild(index).gameObject.GetComponent<ObstacleImage>().id = 0;
    }

    void PushNameTest()
    {
        for(int i = 0; i < 3; i++)
        {
            resultCharacters.transform.GetChild(i).gameObject.transform.GetChild(0).GetComponent<Text>().text = names[i];
        }
    }
}
