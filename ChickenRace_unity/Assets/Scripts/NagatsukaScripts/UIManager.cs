using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject imageObjects;
    public List<int> id;
    [SerializeField, Range(0, 3)] int testSoldOutInex;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeObstacleImage();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SoldOut(testSoldOutInex);
        }
    }

    /// <summary>
    /// GameManager����ID������֐�
    /// </summary>
    public void PushID(List<int> iD)
    {
        id = new List<int>();
        id = iD;
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
            imageObjects.transform.GetChild(i).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(id[i]);//�摜��ύX.
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
            ResorceManager.instance.GetObstacleImage(OBSTACLE_IMAGE_NAMES.Kanbaipop);//�����摜�ɕύX.
        imageObjects.transform.GetChild(index).gameObject.GetComponent<ObstacleImage>().id = 0;
    }
}
