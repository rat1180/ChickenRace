using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject imageObjects;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            ChangeObstacleImage();
        }
    }

    /// <summary>
    /// 4‚Â‚Ì‰æ‘œ‚ð•Ï‚¦‚é
    /// </summary>
    public void ChangeObstacleImage()
    {
        imageObjects.transform.GetChild(0).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(ResorceNames.OBSTACLE_IMAGE_NAMES.cutter);
        imageObjects.transform.GetChild(1).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(ResorceNames.OBSTACLE_IMAGE_NAMES.cutter);
        imageObjects.transform.GetChild(2).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(ResorceNames.OBSTACLE_IMAGE_NAMES.taihou);
        imageObjects.transform.GetChild(3).GetComponent<Image>().sprite =
            ResorceManager.instance.GetObstacleImage(ResorceNames.OBSTACLE_IMAGE_NAMES.blackhole);
        //imageObjects.transform.GetChild(0).GetComponent<Image>().sprite =
        //    ResorceManager.instance.GetObstacleImage(0);
        //imageObjects.transform.GetChild(1).GetComponent<Image>().sprite =
        //    ResorceManager.instance.GetObstacleImage(1);
        //imageObjects.transform.GetChild(2).GetComponent<Image>().sprite =
        //    ResorceManager.instance.GetObstacleImage(2);
        //imageObjects.transform.GetChild(3).GetComponent<Image>().sprite =
        //    ResorceManager.instance.GetObstacleImage(3);
    }
}
