using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ResorceNames;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject imageObjects;
    public List<OBSTACLE_IMAGE_NAMES> id;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeObstacleImage();
        }
    }

    /// <summary>
    /// GameManager‚©‚çID‚ğ“ü‚ê‚éŠÖ”
    /// </summary>
    public void PushID(List<OBSTACLE_IMAGE_NAMES> iD)
    {
        id = new List<OBSTACLE_IMAGE_NAMES>();
        id = iD;
        ChangeObstacleImage();
    }

    /// <summary>
    /// ID‚ÌƒŠƒXƒg‚ğ‰Šú‰»‚·‚é.
    /// </summary>
    public void ResetID()
    {

    }

    /// <summary>
    /// 4‚Â‚Ì‰æ‘œ‚ğ•Ï‚¦‚é
    /// </summary>
    public void ChangeObstacleImage()
    {
        imageObjects.transform.GetChild(0).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(id[0]);
        imageObjects.transform.GetChild(1).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(id[1]);
        imageObjects.transform.GetChild(2).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(id[2]);
        imageObjects.transform.GetChild(3).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(id[3]);
    }
}
