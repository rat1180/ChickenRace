using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using ResorceNames;

public class ResourceManager : MonoBehaviour
{
    //���̃}�l�[�W���[�̃C���X�^���X
    public static ResourceManager instance;

    //���[�h�����I�u�W�F�N�g�̃��X�g(���̂������Ȃ�)
    public Dictionary<OBSTACLE_IMAGE_NAMES, Sprite> obstacle_images;
    public Dictionary<OBSTACLE_OBJECT, GameObject> obstacle_objects;

    //�摜�ǂݍ��ݗp�p�X
    const string OBSTACLE_IMAGES = "Obstacles/";

    #region Unity�C�x���g(Awake)
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);

            LoadResorceObjects();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    /// <summary>
    /// Awake�ŌĂяo���֐��B�K�v�ȃ��\�[�X�����[�h����
    /// ���[�h���郊�\�[�X��ConstList�ɓo�^����Ă��郊�X�g����Q��
    /// ���[�h�����ނ��ꂼ�ꃊ�X�g��p�ӂ���.
    /// </summary>
    public void LoadResorceObjects()
    {
        LoadObstacleImages();
        LoadObstacleObject();
    }

    #region ���[�h�֐�

    /// <summary>
    /// ��Q���̉摜�ǂݍ��ݗp.
    /// </summary>
    void LoadObstacleImages()
    {
        //������
        obstacle_images = new Dictionary<OBSTACLE_IMAGE_NAMES, Sprite>();

        //���O���J��Ԃ�
        foreach (OBSTACLE_IMAGE_NAMES name in Enum.GetValues(typeof(OBSTACLE_IMAGE_NAMES)))
        {
            //�����Ώۂ�T��
             var prefabobj = FolderObjectFinder.LoadObstacleImages(OBSTACLE_IMAGES + name.ToString());

            obstacle_images.Add(name, prefabobj);
        }
    }

    /// <summary>
    /// ��Q���̃I�u�W�F�N�g�ǂݍ��ݗp.
    /// </summary>
    void LoadObstacleObject()
    {
        //������
        obstacle_objects = new Dictionary<OBSTACLE_OBJECT, GameObject>();

        //���O���J��Ԃ�
        foreach (OBSTACLE_OBJECT name in Enum.GetValues(typeof(OBSTACLE_OBJECT)))
        {
            //�����Ώۂ�T��
            var prefabobj = FolderObjectFinder.LoadObstacleObject(name.ToString());

            obstacle_objects.Add(name, prefabobj);
        }
    }

    #endregion

    #region �Q�b�g�֐�

    /// <summary>
    /// ��Q���̉摜�𒼐ږ��O�w�肵�ēǂݍ���.
    /// </summary>
    public Sprite GetObstacleImage(OBSTACLE_IMAGE_NAMES name)
    {
        if (obstacle_images.ContainsKey(name))
        {
            return obstacle_images[name];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽOBSTACLE_IMAGE_NAMES�����X�g�ɂ���܂���");
            return null;
        }
    }

    /// <summary>
    /// ��Q����ID���w�肵�ēǂݍ���.
    /// </summary>
    public Sprite GetObstacleImage(int id)
    {

        //ID�ƈ�v������̂���������
        foreach (OBSTACLE_IMAGE_NAMES name in Enum.GetValues(typeof(OBSTACLE_IMAGE_NAMES)))
        {
            if ((int)name == id)//ID����v.
            {
                if (obstacle_images.ContainsKey(name))
                {
                    return obstacle_images[name];//��v�������̂�����������֐��𔲂���.
                }
            }
        }
        Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽOBSTACLE_IMAGE_NAMES�����X�g�ɂ���܂���");
        return null;//�݂���Ȃ�������null��Ԃ�.
    }

    /// <summary>
    /// ��Q���̃I�u�W�F�N�g�𒼐ږ��O�w�肵�ēǂݍ���.
    /// </summary>
    public GameObject GetObstacleObject(OBSTACLE_OBJECT name)
    {
        if (obstacle_objects.ContainsKey(name))
        {
            return obstacle_objects[name];
        }
        else
        {
            Debug.LogWarning("���\�[�X�擾�G���[�F���͂��ꂽOBSTACLE_OBJECT�����X�g�ɂ���܂���");
            return null;
        }
    }
    #endregion

}

/// <summary>
/// ���\�[�X�̖��O���Q�Ƃ���p
/// ���̖��O��Ԃɓǂݍ��ރ��\�[�X��"�S��"�L������.
/// </summary>
namespace ResorceNames
{ 
    public enum OBSTACLE_IMAGE_NAMES
    {
        Kanbaipop,//�����摜.
        taihou,
        cutter,
        blackhole,
    }

    /// <summary>
    /// ��Q���I�u�W�F�N�g�̖��O�ꗗ
    /// �M�~�b�N�Ȃ��FNormal
    /// �_���[�W�M�~�b�N:Damage
    /// �����M�~�b�N:Move_
    /// </summary>
    public enum OBSTACLE_OBJECT
    {
        Damage_Arrow,
        Damage_Paunch,
        Move_Cannon,
        Move_Normal_Scaffold_Move,
        Move_Normal_Scaffold_Rotate,
        Move_Scaffold_Hole,
        Move_Scaffold_Surprise,
        Move_ZeroGravity,
        Normal_Scaffold,
        Normal_Scaffold_Four,
        Normal_Scaffold_Hole2,
        Normal_Scaffold_L,
        Normal_Scaffold_Square,
        Normal_Scaffold_Stairs,
        Normal_Scaffold_Three,
        Normal_Scaffold_Two,
    }
}

//�I�u�W�F�N�g���Ɛ����p�t�H���_���w�肷�邱�Ƃ�
//���̃t�H���_����v���t�@�u����������
public static class FolderObjectFinder
{
    //�����p�t�H���_�ւ̃p�X
    const string DefalutGenerateFolderName = "Prefabs/";

    //��Q���I�u�W�F�N�g�ւ̃p�X
    const string OBSTACLE_FOLDER = "Obstacle/";

    //�摜�t�H���_�ւ̃p�X
    const string IMAGES_FOLDER = "Images/";

    /// <summary>
    /// �����p�t�H���_����I�u�W�F�N�g��T���A�Ԃ��B
    /// ����������Ȃ���΋�̃Q�[���I�u�W�F�N�g��Ԃ�
    /// �f�t�H���g�̐����t�H���_���w�肳��Ă���̂ŁA�f�t�H���g����̑��ΎQ�ƂŖ��O������
    /// </summary>
    public static GameObject LoadResorceGameObject(string objectname)
    {
        var obj = (GameObject)Resources.Load(DefalutGenerateFolderName + objectname);

        Debug.Log(DefalutGenerateFolderName + objectname);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("���\�[�X�t�@�C����\"" + objectname + "\"��������܂���ł���");
            //�Ԃ�����ŃG���[���N����Ȃ��悤�ɒ��g����̃I�u�W�F�N�g��Ԃ�
            return new GameObject();
        }
    }

    /// <summary>
    /// ��Q�������p�t�H���_����I�u�W�F�N�g��T���A�Ԃ��B
    /// ����������Ȃ���΋�̃Q�[���I�u�W�F�N�g��Ԃ�
    /// </summary>
    public static GameObject LoadObstacleObject(string objectname)
    {
        var obj = (GameObject)Resources.Load(OBSTACLE_FOLDER + objectname);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("���\�[�X�t�@�C����\"" + objectname + "\"��������܂���ł���");
            //�Ԃ�����ŃG���[���N����Ȃ��悤�ɒ��g����̃I�u�W�F�N�g��Ԃ�
            return new GameObject();
        }
    }

    /// <summary>
    /// ��Q���̉摜�ǂݍ���
    /// </summary>
    public static Sprite LoadObstacleImages(string objectname)
    {
        var obj = Resources.Load<Sprite>(IMAGES_FOLDER + objectname);

        //Debug.Log(IMAGES_FOLDER + objectname);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("���\�[�X�t�@�C����\"" + objectname + "\"��������܂���ł���");
            //�Ԃ�����ŃG���[���N����Ȃ��悤�ɒ��g����̃I�u�W�F�N�g��Ԃ�
            return null;
        }
    }

    /// <summary>
    /// ��ނ��킸�A���O�w��ŒT������
    /// �G���[���|���̂ŕK�v�Ȃ����GameObject�w���GetResorceGameObject���g������
    /// </summary>
    public static UnityEngine.Object GetResorceObject(string name)
    {
        var obj = Resources.Load(name);

        Debug.Log(name);
        if (obj != null) return obj;
        else
        {
            Debug.LogError("���\�[�X�t�@�C����\"" + name + "\"��������܂���ł���");
            //�Ԃ�����ŃG���[���N����Ȃ��悤�ɒ��g����̃I�u�W�F�N�g��Ԃ�
            return new UnityEngine.Object();
        }
    }
}