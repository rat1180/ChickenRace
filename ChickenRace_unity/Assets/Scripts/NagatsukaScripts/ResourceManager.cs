using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using ResorceNames;
using SoundName;

public class ResourceManager : MonoBehaviour
{
    //このマネージャーのインスタンス
    public static ResourceManager instance;

    //ロードしたオブジェクトのリスト(実体を持たない)
    public Dictionary<OBSTACLE_IMAGE_NAMES, Sprite> obstacle_images;
    public Dictionary<OBSTACLE_OBJECT, GameObject> obstacle_objects;
    public Dictionary<SECode, AudioClip> se;
    public Dictionary<BGMCode, AudioClip> bgm;

    //画像読み込み用パス
    const string OBSTACLE_IMAGES = "Obstacles/";

    

    #region Unityイベント(Awake・Update)
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

    private void Update()
    {
        ErrorFunction();
    }
    #endregion

    /// <summary>
    /// Awakeで呼び出す関数。必要なリソースをロードする
    /// ロードするリソースはConstListに登録されているリストから参照
    /// ロードする種類それぞれリストを用意する.
    /// </summary>
    public void LoadResorceObjects()
    {
        LoadObstacleImages();
        LoadObstacleObject();
        LoadSE();
        LoadBGM();
    }

    #region ロード関数

    /// <summary>
    /// 障害物の画像読み込み用.
    /// </summary>
    void LoadObstacleImages()
    {
        //初期化
        obstacle_images = new Dictionary<OBSTACLE_IMAGE_NAMES, Sprite>();

        //名前分繰り返し
        foreach (OBSTACLE_IMAGE_NAMES name in Enum.GetValues(typeof(OBSTACLE_IMAGE_NAMES)))
        {
            //生成対象を探索
             var prefabobj = FolderObjectFinder.LoadObstacleImages(OBSTACLE_IMAGES + name.ToString());

            obstacle_images.Add(name, prefabobj);
        }
    }

    /// <summary>
    /// 障害物のオブジェクト読み込み用.
    /// </summary>
    void LoadObstacleObject()
    {
        //初期化
        obstacle_objects = new Dictionary<OBSTACLE_OBJECT, GameObject>();

        //名前分繰り返し
        foreach (OBSTACLE_OBJECT name in Enum.GetValues(typeof(OBSTACLE_OBJECT)))
        {
            //生成対象を探索
            var prefabobj = FolderObjectFinder.LoadObstacleObject(name.ToString());

            obstacle_objects.Add(name, prefabobj);
        }
    }

    /// <summary>
    /// SE読み込み用.
    /// </summary>
    void LoadSE()
    {
        //初期化
        se = new Dictionary<SECode, AudioClip>();

        //名前分繰り返し
        foreach (SECode name in Enum.GetValues(typeof(SECode)))
        {
            //生成対象を探索
            var se_obj = FolderObjectFinder.LoadSE(name.ToString());

            se.Add(name, se_obj);
        }
    }

    /// <summary>
    /// SE読み込み用.
    /// </summary>
    void LoadBGM()
    {
        //初期化
        bgm = new Dictionary<BGMCode, AudioClip>();

        //名前分繰り返し
        foreach (BGMCode name in Enum.GetValues(typeof(BGMCode)))
        {
            //生成対象を探索
            var bgm_obj = FolderObjectFinder.LoadBGM(name.ToString());

            bgm.Add(name, bgm_obj);
        }
    }

    #endregion

    #region ゲット関数

    /// <summary>
    /// 障害物の画像を直接名前指定して読み込む.
    /// </summary>
    public Sprite GetObstacleImage(OBSTACLE_IMAGE_NAMES name)
    {
        if (obstacle_images.ContainsKey(name))
        {
            return obstacle_images[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたOBSTACLE_IMAGE_NAMESがリストにありません");
            return null;
        }
    }

    /// <summary>
    /// 障害物のIDを指定して読み込む.
    /// </summary>
    public Sprite GetObstacleImage(int id)
    {

        //IDと一致するものを検索する
        foreach (OBSTACLE_IMAGE_NAMES name in Enum.GetValues(typeof(OBSTACLE_IMAGE_NAMES)))
        {
            if ((int)name == id)//IDが一致.
            {
                if (obstacle_images.ContainsKey(name))
                {
                    return obstacle_images[name];//一致したものが見つかったら関数を抜ける.
                }
            }
        }
        Debug.LogWarning("リソース取得エラー：入力されたOBSTACLE_IMAGE_NAMESがリストにありません");
        return null;//みつからなかったらnullを返す.
    }

    /// <summary>
    /// 障害物のオブジェクトを直接名前指定して読み込む.
    /// </summary>
    public GameObject GetObstacleObject(OBSTACLE_OBJECT name)
    {
        if (obstacle_objects.ContainsKey(name))
        {
            return obstacle_objects[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたOBSTACLE_OBJECTがリストにありません");
            return null;
        }
    }


    /// <summary>
    /// SEを直接名前指定して読み込む.
    /// </summary>
    public AudioClip GetSE(SECode name)
    {
        if (se.ContainsKey(name))
        {
            return se[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたSEがリストにありません");
            return null;
        }
    }

    /// <summary>
    /// SEを直接名前指定して読み込む.
    /// </summary>
    public AudioClip GetBGM(BGMCode name)
    {
        if (bgm.ContainsKey(name))
        {
            return bgm[name];
        }
        else
        {
            Debug.LogWarning("リソース取得エラー：入力されたBGMがリストにありません");
            return null;
        }
    }

    #endregion


    /// <summary>
    /// なんらかのエラーが発生した際にescキーで強制終了できるようにする関数.
    /// </summary>
    private void ErrorFunction()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard.escapeKey.wasPressedThisFrame)
        {
#if UNITY_EDITOR //UnityEditorで起動しているとき.
            UnityEditor.EditorApplication.isPlaying = false;//ゲームプレイ終了
#else //ビルド環境.
    Application.Quit();//ゲームプレイ終了
#endif
        }
    }
}

/// <summary>
/// リソースの名前を参照する用
/// この名前空間に読み込むリソースを"全て"記入する.
/// </summary>
namespace ResorceNames
{ 
    public enum OBSTACLE_IMAGE_NAMES
    {
        Damage_Arrow = 1,
        Damage_Paunch,
        Move_Cannon,
        Move_Normal_Scaffold_Move,
        Move_Normal_Scaffold_Rotate,
        Move_Scaffold_Hole,
        Move_Scaffold_Surprise,
        Move_ZeroGravity,
        Normal_Scaffold,
        Kanbaipop,//完売画像.
        taihou,
        cutter,
        //blackhole
    }

    /// <summary>
    /// 障害物オブジェクトの名前一覧
    /// ギミックなし：Normal
    /// ダメージギミック:Damage
    /// 動くギミック:Move_
    /// 削除用ID:Destriy_
    /// </summary>
    public enum OBSTACLE_OBJECT
    {
        Damage_Arrow = 1,
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
        Destroy_Bom,
    }

}

//オブジェクト名と生成用フォルダを指定することで
//そのフォルダからプレファブを検索する
public static class FolderObjectFinder
{
    //生成用フォルダへのパス
    const string DefalutGenerateFolderName = "Prefabs/";

    //障害物オブジェクトへのパス
    const string OBSTACLE_FOLDER = "Obstacle/";

    //画像フォルダへのパス
    const string IMAGES_FOLDER = "Images/";

    //音素材フォルダへのパス.
    const string SOUNDS_PASS = "Sounds/";

    /// <summary>
    /// 生成用フォルダからオブジェクトを探し、返す。
    /// もし見つからなければ空のゲームオブジェクトを返す
    /// デフォルトの生成フォルダが指定されているので、デフォルトからの相対参照で名前を入れる
    /// </summary>
    public static GameObject LoadResorceGameObject(string objectname)
    {
        var obj = (GameObject)Resources.Load(DefalutGenerateFolderName + objectname);

        Debug.Log(DefalutGenerateFolderName + objectname);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("リソースファイルに\"" + objectname + "\"が見つかりませんでした");
            //返した先でエラーが起こらないように中身が空のオブジェクトを返す
            return new GameObject();
        }
    }

    /// <summary>
    /// 障害物生成用フォルダからオブジェクトを探し、返す。
    /// もし見つからなければ空のゲームオブジェクトを返す
    /// </summary>
    public static GameObject LoadObstacleObject(string objectname)
    {
        var obj = (GameObject)Resources.Load(OBSTACLE_FOLDER + objectname);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("リソースファイルに\"" + objectname + "\"が見つかりませんでした");
            //返した先でエラーが起こらないように中身が空のオブジェクトを返す
            return new GameObject();
        }
    }

    /// <summary>
    /// 障害物の画像読み込み
    /// </summary>
    public static Sprite LoadObstacleImages(string objectname)
    {
        var obj = Resources.Load<Sprite>(IMAGES_FOLDER + objectname);

        //Debug.Log(IMAGES_FOLDER + objectname);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("リソースファイルに\"" + objectname + "\"が見つかりませんでした");
            //返した先でエラーが起こらないように中身が空のオブジェクトを返す
            return null;
        }
    }

    /// <summary>
    /// SE読み込み
    /// </summary>
    public static AudioClip LoadSE(string name)
    {
        var obj = Resources.Load<AudioClip>(SOUNDS_PASS + "SE/" + name);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("リソースファイルに\"" + name + "\"が見つかりませんでした");
            //返した先でエラーが起こらないように中身が空のオブジェクトを返す
            return null;
        }
    }

    /// <summary>
    /// BGM読み込み
    /// </summary>
    public static AudioClip LoadBGM(string name)
    {
        var obj = Resources.Load<AudioClip>(SOUNDS_PASS + "BGM/" + name);

        if (obj != null) return obj;
        else
        {
            Debug.LogError("リソースファイルに\"" + name + "\"が見つかりませんでした");
            //返した先でエラーが起こらないように中身が空のオブジェクトを返す
            return null;
        }
    }

    /// <summary>
    /// 種類を問わず、名前指定で探索する
    /// エラーが怖いので必要なければGameObject指定のGetResorceGameObjectを使うこと
    /// </summary>
    public static UnityEngine.Object GetResorceObject(string name)
    {
        var obj = Resources.Load(name);

        Debug.Log(name);
        if (obj != null) return obj;
        else
        {
            Debug.LogError("リソースファイルに\"" + name + "\"が見つかりませんでした");
            //返した先でエラーが起こらないように中身が空のオブジェクトを返す
            return new UnityEngine.Object();
        }
    }
}