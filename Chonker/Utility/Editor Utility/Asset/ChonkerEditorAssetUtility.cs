using System.Collections;
using System.Collections.Generic;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class ChonkerEditorAssetUtility : MonoBehaviour
{
    #if UNITY_EDITOR
    public static List<T> searchForAssetOfTypeInFolder<T>(string pathRelativeToAssetsFolder = "")
        where T : UnityEngine.Object {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}", new[] { "Assets/" + pathRelativeToAssetsFolder });
        List<T> foundAssets = new List<T>();
        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T foundAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            foundAssets.Add(foundAsset);
        }

        return foundAssets;
    }

    public static List<T> searchForPrefabsOfTypeInFolder<T>(string pathRelativeToAssetsFolder = "") where T : MonoBehaviour {
        string[] guids = AssetDatabase.FindAssets($"t:prefab", new[] { "Assets/" + pathRelativeToAssetsFolder });
        List<T> foundAssets = new List<T>();
        foreach (string guid in guids) {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            T foundAsset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (foundAsset == null) {
                continue;
            }
            foundAssets.Add(foundAsset);
        }

        return foundAssets;
    }

    public static T createScriptableObject<T>() where T : ScriptableObject {
        EditorUtility.FocusProjectWindow();

        if (!TryGetActiveFolderPath(out var path)) {
            path = "Assets";
        }

        return createScriptableObject<T>(path);
    }

    public static T createScriptableObject<T>(string assetPath) where T : ScriptableObject {
        return createScriptableObjectWithLocation<T>(assetPath, typeof(T).Name);
    }

    private static T createScriptableObjectWithLocation<T>(string location, string name)
        where T : ScriptableObject {
        T asset = ScriptableObject.CreateInstance<T>();

        AssetDatabase.CreateAsset(asset, $"{location}/{name}.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
        return asset;
    }

    public static bool TryGetActiveFolderPath(out string path) {
        var _tryGetActiveFolderPath = typeof(ProjectWindowUtil).GetMethod("TryGetActiveFolderPath",
            BindingFlags.Static | BindingFlags.NonPublic);

        object[] args = new object[] { null };
        bool found = (bool)_tryGetActiveFolderPath.Invoke(null, args);
        path = (string)args[0];

        return found;
    }
    
    #endif
}
