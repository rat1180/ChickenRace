using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouse : MonoBehaviour
{
    [SerializeField] Vector3 moveVector;     // InputActionから受け取った値を入れる.
    [SerializeField] float moveSpeed;        // 動く速さ.
    [SerializeField] int itemId;
    [SerializeField] int index;             // アイテム番号.
    int error;                              // エラー番号.
    [SerializeField] bool isInstalled;         // アイテムの設置が可能か.
    [SerializeField] GameObject mouseImage;  // 自身の画像.
    [SerializeField] GameObject instanceObj; // 生成した画像.
    [SerializeField] GameObject map;
    [SerializeField] GameObject user;
    Vector2Int gridPos;

    [SerializeField] float angle;
    [SerializeField] float saveAngle;

    /// <summary>
    /// 初期化用関数.
    /// </summary>
    private void Init()
    {
        error = -1;
        ImageInstance();
        //map = GameManager.instance.GetMapManager();
        map = GameObject.Find("MapManager");
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
        isInstalled = map.GetComponent<MapManager>().JudgeInstall(gridPos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "SelectImage") return;
        // 当たった画像のIDを取得.
        itemId = collision.gameObject.GetComponent<ObstacleImage>().ReturnID();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        itemId = error;
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
        Debug.Log("クリック");

        //if (User.GetComponent<User>().SetMode() == false)
        //{
        //    // 選択フェーズ.
        //GameManager.Instance.MouseSelected(index);
           
        //}
        //else
        //{
        //    // 設置フェーズ.
        //}

        // アイテムが設置可能なら.
        if (isInstalled == false)
        {
            // アイテムの生成.
           map.GetComponent<MapManager>().GenerateMapObject(0,saveAngle, gridPos);
        }
        else
        {
            // Debug.Log("設置できません");
        }

        if (itemId != error)
        {
            index = itemId; // itemIdを関数にする.
            user.GetComponent<User>().SetIndex(index);
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

    /// <summary>
    /// マウスのポジションセット.
    /// </summary>
    private void MouseTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }

    /// <summary>
    /// オブジェクト左回転用
    /// </summary>
    private void OnLeftRotate()
    {
        saveAngle += angle;
    }

    /// <summary>
    /// オブジェクト右回転用.
    /// </summary>
    private void OnRightRotate()
    {
        saveAngle -= angle;
    }

    public void SetUser(User setuser)
    {
        user = setuser.gameObject;
    }

    
}