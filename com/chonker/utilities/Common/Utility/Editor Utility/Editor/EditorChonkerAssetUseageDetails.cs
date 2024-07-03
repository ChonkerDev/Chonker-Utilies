using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
namespace Chonker.Utility.Editor_Utility.Editor
{
    public class EditorChonkerAssetUseageDetails : ScriptableObject
    {
        public string chonkerNameForPack;
        public string assetPackrUrl;
        public string proofOfAllowedUseInGames;
        public UsageAllowedType usageAllowedType;
        public string usageAllowedNotes;
        [TextArea(1, 30)]public string assetNotes;

        public enum UsageAllowedType
        {
            Approved,
            Unknown,
            NotAllowed
        }

        [MenuItem("Chonker/Create Scriptable Object/MetaData/Asset Useage Details")]
        private static void createAsset() {
            ChonkerEditorAssetUtility.createScriptableObject<EditorChonkerAssetUseageDetails>();
        }
    
        [CustomEditor(typeof(EditorChonkerAssetUseageDetails))]
        public class EditorChonkerAssetUseageDetailsEditor : UnityEditor.Editor
        {
            private SerializedProperty chonkerNameForPack, assetPackrUrl, proofOfAllowedUseInGames, assetNotes, usageAllowedNotes, usageAllowedType;

            private void OnEnable() {
                assetPackrUrl = serializedObject.FindProperty("assetPackrUrl");
                proofOfAllowedUseInGames = serializedObject.FindProperty("proofOfAllowedUseInGames");
                assetNotes = serializedObject.FindProperty("assetNotes");
                chonkerNameForPack = serializedObject.FindProperty("chonkerNameForPack");
                usageAllowedType = serializedObject.FindProperty("usageAllowedType");
                usageAllowedNotes = serializedObject.FindProperty("usageAllowedNotes");

            }

            public override void OnInspectorGUI() {
                EditorGUILayout.PropertyField(chonkerNameForPack);
                if (usageAllowedType.enumValueIndex == (int)UsageAllowedType.NotAllowed) {
                    EditorGUILayout.HelpBox("This is asset pack is marked as not allowed to be used in game. Delete it from the project", MessageType.Error);
                }
            
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(assetPackrUrl);
                if (GUILayout.Button("Go To Asset Page")) {
                    Application.OpenURL(assetPackrUrl.stringValue);
                }


                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(usageAllowedType);
                EditorGUILayout.PropertyField(usageAllowedNotes);
                EditorGUILayout.PropertyField(proofOfAllowedUseInGames);
                EditorGUILayout.PropertyField(assetNotes);
                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            
            }
        
        
        }
    }
}
#endif
