using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    Gamepad gamepad;
    [SerializeField] Vector2 inputControl;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
    /// コントローラーの左スティックの値を取得.
    /// </summary>
    /// <param name="value"></param>
    void OnControllerValue(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        inputControl = new Vector3(velocity.x, velocity.y, 0);
    }

    /// <summary>
    /// 決定ボタンが押されたとき.
    /// </summary>
    void OnCircleButtonPush()
    {
        
    }
}
