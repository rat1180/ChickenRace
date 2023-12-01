using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obstacle : MonoBehaviour
{
    protected Vector2Int ObstacleCenterPos; // ���S�O���b�h�ʒu
    [SerializeField]
    List<Vector2Int> CollisionList; // ���΃O���b�h

    [SerializeField]
    int ObstacleID; // ��Q����ID
    [SerializeField]
    int ObstacleKindID; // ��ނ�ID
    [SerializeField]
    int MyRotation; // ��]
    [SerializeField]
    UnityEvent UE; // �C�x���g�֐�
    void Awake()
    {
        Init();
    }
    /// <summary>
    /// ������
    /// </summary>
    public virtual void Init()
    {
        ObstacleCenterPos = new Vector2Int(0, 0);
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
        MyRotation = rot;
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
        return CollisionList;
    }
    public virtual void ObjStart()
    {

    }
}
