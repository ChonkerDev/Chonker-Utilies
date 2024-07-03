using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Chonker.Utility.Editor_Utility.Editor
{
    public class EditorChonkerAssetUsageWindow : EditorWindow
    {
        private static EditorChonkerAssetUseageDetails[] usageDetailsFound;
        [MenuItem("Chonker/Find Created Asset Usages")]
        private static void showWindow() {
            EditorChonkerAssetUsageWindow wnd = GetWindow<EditorChonkerAssetUsageWindow>();
            wnd.titleContent = new GUIContent("Chonker Asset Usage");
        }

        private void OnGUI() {
            usageDetailsFound = ChonkerEditorAssetUtility.searchForAssetOfTypeInFolder<EditorChonkerAssetUseageDetails>().ToArray();
            foreach (var editorChonkerAssetUseageDetails in usageDetailsFound) {
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(editorChonkerAssetUseageDetails.name);
                GUI.enabled = false;
                EditorGUILayout.ObjectField(editorChonkerAssetUseageDetails, typeof(EditorChonkerAssetUseageDetails));
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
        }
    }
}

#endif
