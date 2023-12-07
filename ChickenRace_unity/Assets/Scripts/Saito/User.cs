using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject mouse;

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

    private void GenerateMouse()
    {
        Instantiate(mouse, transform.position, transform.rotation);
    }
}
