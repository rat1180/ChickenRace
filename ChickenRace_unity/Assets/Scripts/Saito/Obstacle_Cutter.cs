using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle_Cutter : Obstacle
{
    float elasticitySpeed;  // �J�b�^�[�̐L�k�̕�.
    GameObject Blade;
    GameObject MaxBladePos;
    [SerializeField] bool isTurn;  // �E�ɐi�ނ����ɐi�ނ�.
    Vector3 BladeStartPos;

    public override void Init()
    {
        elasticitySpeed = 2.0f;
        Blade = transform.GetChild(0).gameObject;
        MaxBladePos = transform.GetChild(1).gameObject;
        BladeStartPos = Blade.transform.position;
    }

    public override void update()
    {
        Elasticity();
    }

    /// <summary>
    /// �J�b�^�[�̐L�k����.
    /// </summary>
    /// <returns></returns>
    void Elasticity()
    {
       var dis = Vector3.Distance(BladeStartPos, Blade.transform.position);
       var maxdis = Vector3.Distance(BladeStartPos, MaxBladePos.transform.position);

        if (dis > maxdis)
        {
            isTurn = false;
        }
        else if(dis <= 0.1f)
        {
            isTurn = true;
        }

        if (isTurn == true)
        {
            // Blade�I�u�W�F�N�g�������̌����ɓ�����.
            Blade.transform.position += transform.TransformDirection(Vector3.right) * elasticitySpeed * Time.deltaTime;

        }
        else
        {
            Blade.transform.position -= transform.TransformDirection(Vector3.right) * elasticitySpeed * Time.deltaTime;
        }
    }
}
