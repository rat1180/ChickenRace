using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;
using PhotonMethods;

public class PlayerMouse : MonoBehaviour
{
    [SerializeField] Vector3 moveVector;     // InputAction����󂯎�����l������.
    [SerializeField] float moveSpeed;        // ��������.
    [SerializeField] int itemId;
    [SerializeField] int index;              // �A�C�e���ԍ�.
    int error;                               // �G���[�ԍ�.
    [SerializeField] bool isInstalled;       // �A�C�e���̐ݒu���\��.
    [SerializeField] GameObject mouseImage;  // ���g�̉摜.
    [SerializeField] GameObject mouseInstanceObj; // ���������摜.
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
        MouseImageInstance();
        map = GameManager.instance.GetMapManager();
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
        // Debug.Log("�N���b�N");

        // �A�C�e�����ݒu�\�Ȃ�.
        if (isInstalled == true)
        {
            // �A�C�e���̐���.
           map.GetComponent<MapManager>().GenerateMapObject(0,saveAngle, gridPos);
        }
        else
        {
            // Debug.Log("�ݒu�ł��܂���");
            CantPlant();

        }

        if (itemId != error)
        {
            index = itemId; // itemId���֐��ɂ���.
            user.GetComponent<User>().SetIndex(index);
        }
    }

    /// <summary>
    /// �}�E�X�摜�̐���.
    /// </summary>
    private void MouseImageInstance()
    {
        // mouseImage = Instantiate(mouseImage, transform.position, transform.rotation);
        mouseImage = "mouseImage".SafeInstantiate(transform.position, transform.rotation);
    }

    /// <summary>
    /// �}�E�X�̃|�W�V�����Z�b�g.
    /// </summary>
    private void MouseTransform()
    {
        mouseImage.GetComponent<Character>().PositionUpdate(transform.position);
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

    /// <summary>
    /// �ݒu�o���Ȃ��������ɌĂ�.
    /// </summary>
    private void CantPlant()
    {

    }

    /// <summary>
    /// �ݒu�t�F�[�Y�ɌĂԊ֐�.
    /// </summary>
    public void PlantPhase()
    {
        gridPos = new Vector2Int(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y));

        if (itemId != error)
        {
            isInstalled = map.GetComponent<MapManager>().JudgeInstall(gridPos, itemId);
        }
    }
    
}