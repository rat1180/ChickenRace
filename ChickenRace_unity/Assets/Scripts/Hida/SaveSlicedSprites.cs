using UnityEngine;
using UnityEditor;

public class SaveSlicedSprites : EditorWindow
{
    [MenuItem("Tools/Save Sliced Sprites")]
    static void Init()
    {
        SaveSlicedSprites window = (SaveSlicedSprites)EditorWindow.GetWindow(typeof(SaveSlicedSprites));
        window.Show();
    }

    void OnGUI()
    {
        if (GUILayout.Button("Save Sliced Sprites"))
        {
            SaveSprites();
        }
    }

    void SaveSprites()
    {
        // �X���C�X���ꂽ�X�v���C�g���܂ރX�v���C�g�V�[�g�̃p�X
        string spriteSheetPath = "Assets/Resources/Images/Obstacles/Scaffold.png";

        // �X���C�X���ꂽ�X�v���C�g��Prefab��ۑ�����f�B���N�g���̃p�X
        string prefabSavePath = "Assets/Resources/Sprites/SlicedSprites/";

        // �X�v���C�g�V�[�g�����[�h
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);

        // �X���C�X���ꂽ�X�v���C�g��Prefab�Ƃ��ĕۑ�
        foreach (Object sprite in sprites)
        {
            if (sprite is Sprite)
            {
                // �X�v���C�g��Prefab���쐬
                GameObject spritePrefab = new GameObject(sprite.name);
                spritePrefab.AddComponent<SpriteRenderer>().sprite = (Sprite)sprite;

                // Prefab��ۑ�
                string prefabPath = prefabSavePath + sprite.name + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(spritePrefab, prefabPath);
                DestroyImmediate(spritePrefab);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Sliced sprites saved as Prefabs.");
    }
}
