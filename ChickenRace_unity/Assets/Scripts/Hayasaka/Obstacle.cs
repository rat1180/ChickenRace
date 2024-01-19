using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int obstacleCenterPos;   // ���S�O���b�h�ʒu
    [SerializeField]
    protected List<Vector2Int> collisionList; // ���΃O���b�h

    [SerializeField]
    protected int obstacleID; // ��Q����ID
    [SerializeField]
    protected int obstacleKindID; // ��ނ�ID
    [SerializeField]
    protected float myRotation; // �������̊p�x���󂯎��
    [SerializeField]
    protected UnityEvent ue;    // �C�x���g�֐�
    void Awake()
    {
        Init();
    }
    /// <summary>
    /// ������
    /// </summary>
    protected virtual void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.CheckObstacleMove())
        {
            update();
        }
        if (GameManager.instance.CheckRaceEnd())
        {
            Destoroy();
        }
    }
    protected virtual void update()
    {
        
    }
    /// <summary>
    /// �������̉�]���̏��󂯎��
    /// </summary>
    /// <param name="rot"></param>
    protected void Generation(int rot)
    {
        myRotation = rot;
    }
    /// <summary>
    /// �j�󂳂��
    /// </summary>
    void Destoroy()
    {
        //this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
    /// <summary>
    /// ���X�g�擾
    /// </summary>
    /// <returns></returns>
    public List<Vector2Int> GetCollisionList()
    {
        return collisionList;
    }
    protected virtual void ObjStart()
    {

    }
}
