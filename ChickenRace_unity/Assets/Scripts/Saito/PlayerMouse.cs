using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouse : MonoBehaviour
{
    [SerializeField] Vector3 moveVector;     // InputActionから受け取った値を入れる.
    [SerializeField] float moveSpeed;        // 動く速さ.
    [SerializeField] int itemId;             // アイテム番号.
    [SerializeField] bool isInstalled;         // アイテムの設置が可能か.
    [SerializeField] GameObject mouseImage;  // 自身の画像.
    [SerializeField] GameObject instanceObj; // 生成した画像.
    [SerializeField] GameObject Map;
    Vector2Int gridPos;

    /// <summary>
    /// 初期化用関数.
    /// </summary>
    private void Init()
    {
        ImageInstance();
    }

    void Start()
    {
        Init();
    }

    
    void FixedUpdate()
    {
        MouseMove();
        MouseTransform();
        gridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));
        isInstalled = Map.GetComponent<MapManager>().JudgeInstall(gridPos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        
    }

    /// <summary>
    /// InputActionから値を受け取る.
    /// </summary>
    /// <param name="value"></param>
    private void OnMove(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        moveVector = new Vector3(velocity.x, velocity.y, 0);
    }

    /// <summary>
    /// ゲーム内のマウスを動かす処理.
    /// </summary>
    private void MouseMove()
    {
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 指定したボタンとマウスのクリックがされたときに呼ぶ関数.
    /// </summary>
    private void OnClick()
    {
        // Debug.Log("クリック");
        // アイテムが設置可能なら.
        if (isInstalled == false)
        {
            // アイテムの生成.
           Map.GetComponent<MapManager>().GenerateMapObject(0, gridPos);
        }
        else
        {
            Debug.Log("設置できません");
        }
    }

    /// <summary>
    /// 画像の生成.
    /// </summary>
    private void ImageInstance()
    {
        instanceObj = Instantiate(mouseImage, transform.position, transform.rotation);
        //instanceObj = PhotonNetwork.Instantiate("MouseImage", transform.position, transform.rotation);
    }

    private void MouseTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }
}
