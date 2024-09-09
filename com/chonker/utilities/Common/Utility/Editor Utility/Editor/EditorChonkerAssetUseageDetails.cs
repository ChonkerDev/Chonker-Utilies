using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

namespace Chonker.Utility.Editor_Utility.Editor
{
    public class EditorChonkerAssetUseageDetails : ScriptableObject
    {
        public bool IsThisUsed;
        public string chonkerNameForPack;
        public string assetPackrUrl;
        public string proofOfAllowedUseInGames;
        public Sprite Icon;
        public string CreatorName;
        public string AssetName;
        public string PurposeDescription;
        public UsageAllowedType usageAllowedType;
        public string usageAllowedNotes;
        [TextArea(1, 30)] public string assetNotes;

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
            private SerializedProperty chonkerNameForPack,
                assetPackrUrl,
                proofOfAllowedUseInGames,
                assetNotes,
                usageAllowedNotes,
                usageAllowedType,
                Icon,
                CreatorName,
                AssetName,
                PurposeDescription,
                IsThisUsed;

            private void OnEnable() {
                assetPackrUrl = serializedObject.FindProperty("assetPackrUrl");
                proofOfAllowedUseInGames = serializedObject.FindProperty("proofOfAllowedUseInGames");
                assetNotes = serializedObject.FindProperty("assetNotes");
                chonkerNameForPack = serializedObject.FindProperty("chonkerNameForPack");
                usageAllowedType = serializedObject.FindProperty("usageAllowedType");
                usageAllowedNotes = serializedObject.FindProperty("usageAllowedNotes");
                Icon = serializedObject.FindProperty("Icon");
                CreatorName = serializedObject.FindProperty("CreatorName");
                AssetName = serializedObject.FindProperty("AssetName");
                PurposeDescription = serializedObject.FindProperty("PurposeDescription");
                IsThisUsed = serializedObject.FindProperty("IsThisUsed");
            }

            public override void OnInspectorGUI() {
                EditorGUILayout.PropertyField(chonkerNameForPack);
                if (usageAllowedType.enumValueIndex == (int)UsageAllowedType.NotAllowed) {
                    EditorGUILayout.HelpBox(
                        "This is asset pack is marked as not allowed to be used in game. Delete it from the project",
                        MessageType.Error);
                }

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(assetPackrUrl);
                if (GUILayout.Button("Go To Asset Page")) {
                    Application.OpenURL(assetPackrUrl.stringValue);
                }


                EditorGUILayout.EndHorizontal();
                EditorGUILayout.PropertyField(IsThisUsed);
                EditorGUILayout.PropertyField(usageAllowedType);
                EditorGUILayout.PropertyField(usageAllowedNotes);
                EditorGUILayout.PropertyField(proofOfAllowedUseInGames);
                EditorGUILayout.PropertyField(Icon);
                EditorGUILayout.PropertyField(CreatorName);
                EditorGUILayout.PropertyField(AssetName);
                EditorGUILayout.PropertyField(PurposeDescription);
                EditorGUILayout.PropertyField(assetNotes);


                serializedObject.ApplyModifiedProperties();

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
            }
        }
    }
}
#endif