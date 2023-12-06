using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    protected Vector2Int obstacleCenterPos; // ���S�O���b�h�ʒu
    [SerializeField]
    List<Vector2Int> collisionList; // ���΃O���b�h

    [SerializeField]
    int obstacleID; // ��Q����ID
    [SerializeField]
    int obstacleKindID; // ��ނ�ID
    [SerializeField]
    int myRotation; // ��]
    [SerializeField]
    UnityEvent ue; // �C�x���g�֐�
    void Awake()
    {
        Init();
    }
    /// <summary>
    /// ������
    /// </summary>
    public virtual void Init()
    {
        obstacleCenterPos = new Vector2Int(0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        update();
    }
    public virtual void update()
    {
        
    }
    /// <summary>
    /// �������̉�]���̏��󂯎��
    /// </summary>
    /// <param name="rot"></param>
    public void Generation(int rot)
    {
        myRotation = rot;
    }
    /// <summary>
    /// �j��
    /// </summary>
    void Destoroy()
    {
        this.gameObject.SetActive(false);
        // Destroy(this.gameObject);
    }
    List<Vector2Int> Seter()
    {
        return collisionList;
    }
    public virtual void ObjStart()
    {

    }
}
