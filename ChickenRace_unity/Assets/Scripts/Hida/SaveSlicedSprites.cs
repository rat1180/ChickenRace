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
        // スライスされたスプライトを含むスプライトシートのパス
        string spriteSheetPath = "Assets/Resources/Images/Obstacles/Scaffold.png";

        // スライスされたスプライトのPrefabを保存するディレクトリのパス
        string prefabSavePath = "Assets/Resources/Sprites/SlicedSprites/";

        // スプライトシートをロード
        Object[] sprites = AssetDatabase.LoadAllAssetsAtPath(spriteSheetPath);

        // スライスされたスプライトをPrefabとして保存
        foreach (Object sprite in sprites)
        {
            if (sprite is Sprite)
            {
                // スプライトのPrefabを作成
                GameObject spritePrefab = new GameObject(sprite.name);
                spritePrefab.AddComponent<SpriteRenderer>().sprite = (Sprite)sprite;

                // Prefabを保存
                string prefabPath = prefabSavePath + sprite.name + ".prefab";
                PrefabUtility.SaveAsPrefabAsset(spritePrefab, prefabPath);
                DestroyImmediate(spritePrefab);
            }
        }

        AssetDatabase.Refresh();
        Debug.Log("Sliced sprites saved as Prefabs.");
    }
}
