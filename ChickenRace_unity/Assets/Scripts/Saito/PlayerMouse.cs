using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouse : MonoBehaviour
{
    [SerializeField] Vector3 moveVector;     // InputAction����󂯎�����l������.
    [SerializeField] float moveSpeed;        // ��������.
    [SerializeField] int itemId;
    [SerializeField] int index;             // �A�C�e���ԍ�.
    int error;                              // �G���[�ԍ�.
    [SerializeField] bool isInstalled;         // �A�C�e���̐ݒu���\��.
    [SerializeField] GameObject mouseImage;  // ���g�̉摜.
    [SerializeField] GameObject instanceObj; // ���������摜.
    [SerializeField] GameObject map;
    [SerializeField] GameObject user;
    Vector2Int gridPos;

    [SerializeField] float angle;
    [SerializeField] float saveAngle;

    /// <summary>
    /// �������p�֐�.
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
        // ���������摜��ID���擾.
        itemId = collision.gameObject.GetComponent<ObstacleImage>().ReturnID();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        itemId = error;
    }

    /// <summary>
    /// InputAction����l���󂯎��.
    /// </summary>
    /// <param name="value"></param>
    private void OnMove(InputValue value)
    {
        var velocity = value.Get<Vector2>();
        moveVector = new Vector3(velocity.x, velocity.y, 0);
    }

    /// <summary>
    /// �Q�[�����̃}�E�X�𓮂�������.
    /// </summary>
    private void MouseMove()
    {
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// �w�肵���{�^���ƃ}�E�X�̃N���b�N�����ꂽ�Ƃ��ɌĂԊ֐�.
    /// </summary>
    private void OnClick()
    {
        Debug.Log("�N���b�N");

        //if (User.GetComponent<User>().SetMode() == false)
        //{
        //    // �I���t�F�[�Y.
        //GameManager.Instance.MouseSelected(index);
           
        //}
        //else
        //{
        //    // �ݒu�t�F�[�Y.
        //}

        // �A�C�e�����ݒu�\�Ȃ�.
        if (isInstalled == false)
        {
            // �A�C�e���̐���.
           map.GetComponent<MapManager>().GenerateMapObject(0,saveAngle, gridPos);
        }
        else
        {
            // Debug.Log("�ݒu�ł��܂���");
        }

        if (itemId != error)
        {
            index = itemId; // itemId���֐��ɂ���.
            user.GetComponent<User>().SetIndex(index);
        }
    }

    /// <summary>
    /// �摜�̐���.
    /// </summary>
    private void ImageInstance()
    {
        instanceObj = Instantiate(mouseImage, transform.position, transform.rotation);
        //instanceObj = PhotonNetwork.Instantiate("MouseImage", transform.position, transform.rotation);
    }

    /// <summary>
    /// �}�E�X�̃|�W�V�����Z�b�g.
    /// </summary>
    private void MouseTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }

    /// <summary>
    /// �I�u�W�F�N�g����]�p
    /// </summary>
    private void OnLeftRotate()
    {
        saveAngle += angle;
    }

    /// <summary>
    /// �I�u�W�F�N�g�E��]�p.
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