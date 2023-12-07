using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject mouse;
    bool isMode;

    void Start()
    {
        
    }


    void Update()
    {
        
    }

    private void GeneratePlayer()
    {
        Instantiate(player, transform.position, transform.rotation);
    }

    private void GenerateMouse(bool mode)
    {
        isMode = mode;
        Instantiate(mouse, transform.position, transform.rotation);
    }

    public bool SetMode()
    {
        return isMode;
    }
}
