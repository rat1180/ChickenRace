using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMouse : MonoBehaviour
{
    [SerializeField] Vector3 moveVector;     // InputAction����󂯎�����l������.
    [SerializeField] float moveSpeed;        // ��������.
    [SerializeField] int itemId;             // �A�C�e���ԍ�.
    [SerializeField] bool isInstalled;         // �A�C�e���̐ݒu���\��.
    [SerializeField] GameObject mouseImage;  // ���g�̉摜.
    [SerializeField] GameObject instanceObj; // ���������摜.
    [SerializeField] GameObject Map;
    Vector2Int gridPos;

    /// <summary>
    /// �������p�֐�.
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
        if (isInstalled == false)
        {
            // �A�C�e���̐���.
           Map.GetComponent<MapManager>().GenerateMapObject(0, gridPos);
        }
        else
        {
            Debug.Log("�ݒu�ł��܂���");
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

    private void MouseTransform()
    {
        instanceObj.GetComponent<Character>().PositionUpdate(transform.position);
    }
}
