using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using PhotonMethods;

public class PlayerMouse : MonoBehaviour
{
    [SerializeField] Vector3 moveVector;     // InputActionから受け取った値を入れる.
    [SerializeField] float moveSpeed;        // 動く速さ.
    [SerializeField] int itemId;
    [SerializeField] int index;              // アイテム番号.
    int error;                               // エラー番号.
    [SerializeField] bool isInstalled;       // アイテムの設置が可能か.
    [SerializeField] GameObject mouseImage;  // 自身の画像.
    [SerializeField] GameObject map;
    [SerializeField] GameObject user;
    Vector2Int gridPos;

    [SerializeField] float angle;
    [SerializeField] float saveAngle;

    [SerializeField] Sprite spriteImage;
    [SerializeField] Color saveColor;

    /// <summary>
    /// 初期化用関数.
    /// </summary>
    private void Init()
    {
        error = -1;
        MouseImageInstance();
        //map = GameManager.instance.GetMapManager();
        map = GameObject.Find("MapManager");
        itemId = user.GetComponent<User>().GetItemId();
    }

    void Start()
    {
        Init();
    }

    
    void FixedUpdate()
    {
        MouseMove();
        MouseTransform();

        // アイテム選択フェーズ.
        if(user.GetComponent<User>().SetMode() == 0)
        {

        }
        // アイテム設置フェーズ.
        else
        {
            PlantPhase();
            NotPlant();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag != "SelectImage") return;
        // 当たった画像のIDを取得.
        itemId = collision.gameObject.GetComponent<ObstacleImage>().ReturnID();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(user.GetComponent<User>().SetMode() == 0)
        {
            itemId = error;
        }
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
        if (isInstalled == true)
        {
            // アイテムの生成.
            map.GetComponent<MapManager>().GenerateMapObject(itemId, saveAngle, gridPos);

            PlantPhase();
            ImageDelete();
        }
        else
        {
            // Debug.Log("設置できません");
            CantPlant();
        }

        // アイテム選択フェーズ.
        if (user.GetComponent<User>().SetMode() == 0)
        {
            ImageDelete();
        }

        if (itemId != error)
        {
            index = itemId; // itemIdを関数にする.
            user.GetComponent<User>().SetIndex(index);
        }
    }

    /// <summary>
    /// マウス画像の生成.
    /// </summary>
    private void MouseImageInstance()
    {
        // mouseImage = Instantiate(mouseImage, transform.position, transform.rotation);
        mouseImage = "mouseImage".SafeInstantiate(transform.position, transform.rotation);
        saveColor = mouseImage.GetComponent<SpriteRenderer>().color;
    }

    /// <summary>
    /// マウスのポジションセット.
    /// </summary>
    private void MouseTransform()
    {
        if(mouseImage != null)
        {
            mouseImage.GetComponent<Character>().PositionUpdate(transform.position);
        }
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

    /// <summary>
    /// 設置出来なかった時に呼ぶ.
    /// </summary>
    private void CantPlant()
    {

    }

    /// <summary>
    /// 設置フェーズに呼ぶ関数.
    /// </summary>
    public void PlantPhase()
    {
       
        gridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if (itemId != error)
        {
            isInstalled = map.GetComponent<MapManager>().JudgeInstall(gridPos, itemId);
        }
    }

    /// <summary>
    /// アイテム画像の表示.
    /// </summary>
    public void ImageDisplay(Sprite sprite)
    {
        mouseImage.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void ImageDelete()
    {
        Destroy(mouseImage);
    }

    /// <summary>
    /// マウスの初期化.
    /// </summary>
    public void MouseInit(float itemsize)
    {
        // 画像の色を戻す.
        mouseImage.GetComponent<SpriteRenderer>().color = saveColor;

        // 画像のサイズ変更.
        mouseImage.transform.localScale = new Vector3(itemsize, itemsize, itemsize);

    }

    /// <summary>
    /// 設置できないときに呼ぶ関数.
    /// </summary>
    void NotPlant()
    {
        // 設置ができない場合は画像を赤くする.
        if (isInstalled)
        {
            mouseImage.GetComponent<SpriteRenderer>().color = saveColor;
        }
        else
        {
            mouseImage.GetComponent<SpriteRenderer>().color = new Color(255, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a);
        }
    }

}