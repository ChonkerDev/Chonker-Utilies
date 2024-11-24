using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class EditorBetterLitShaderConverter : EditorWindow
{
    [MenuItem("Chonker/Better Lit Shader Converter")]
    public static void ShowWindow() {
        EditorBetterLitShaderConverter windows = GetWindow<EditorBetterLitShaderConverter>();
        windows.titleContent = new GUIContent("Better Lit Shader Converter");
    }


    private ConversionTypeE ConversionType = ConversionTypeE.OneToOne;

    //One to One mode Props
    private MaterialListWrapper sourceUrpMats;
    private MaterialListWrapper newBetterLitMats;

    //Folder Copy Mode
    private MaterialWrapper BaseMaterial;
    private FolderAssetWrapper SourceFolder;
    private FolderAssetWrapper DestinationFolder;

    private void OnEnable() {
        // Create a transient instance of MaterialListWrapper
        sourceUrpMats = CreateInstance<MaterialListWrapper>();
        newBetterLitMats = CreateInstance<MaterialListWrapper>();

        BaseMaterial = CreateInstance<MaterialWrapper>();
        SourceFolder = CreateInstance<FolderAssetWrapper>();
        DestinationFolder = CreateInstance<FolderAssetWrapper>();
    }

    private void OnGUI() {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Folder Copy Mode")) {
            ConversionType = ConversionTypeE.FolderCopy;
        }
        else if (GUILayout.Button("Material Property Copy Mode")) {
            ConversionType = ConversionTypeE.OneToOne;
        }

        EditorGUILayout.EndHorizontal();


        if (GUILayout.Button("Copy Textures")) {
            if (ConversionType == ConversionTypeE.OneToOne) {
                CopyTexturesOneToOne();
            }
            else if (ConversionType == ConversionTypeE.FolderCopy) {
                CopyMatPropertiesToFolder();
            }
        }

        switch (ConversionType) {
            case ConversionTypeE.OneToOne:
                EditorGUILayout.HelpBox("Material Lists must match One to One and must be in the correct order",
                    MessageType.Info);
                GUILayout.BeginHorizontal();
                displayMats(ref sourceUrpMats, "Source Materials");
                displayMats(ref newBetterLitMats, "New Materials");
                GUILayout.EndHorizontal();
                break;
            case ConversionTypeE.FolderCopy: {
                SerializedObject baseMaterialSerializedObject = new SerializedObject(BaseMaterial);
                SerializedProperty BaseMat = baseMaterialSerializedObject.FindProperty("material");
                baseMaterialSerializedObject.Update();
                EditorGUILayout.PropertyField(BaseMat, new GUIContent("Base Better Lit Material"), true);
                baseMaterialSerializedObject.ApplyModifiedProperties();

                SerializedObject sourceFolderSerializedObject = new SerializedObject(SourceFolder);
                SerializedProperty sourceFolder = sourceFolderSerializedObject.FindProperty("Folder");
                sourceFolderSerializedObject.Update();
                EditorGUILayout.PropertyField(sourceFolder, new GUIContent("Source Folder"), true);
                sourceFolderSerializedObject.ApplyModifiedProperties();

                SerializedObject destinationFolderSerializedObject = new SerializedObject(DestinationFolder);
                SerializedProperty destFolder = destinationFolderSerializedObject.FindProperty("Folder");
                destinationFolderSerializedObject.Update();
                EditorGUILayout.PropertyField(destFolder, new GUIContent("Destination Folder"), true);
                destinationFolderSerializedObject.ApplyModifiedProperties();
                break;
            }
        }
    }

    private void CopyTexturesOneToOne() {
        if (sourceUrpMats.materials.Length != newBetterLitMats.materials.Length) {
            Debug.LogError("Material Lists must be the same length");
            return;
        }

        for (var i = 0; i < sourceUrpMats.materials.Length; i++) {
            Material sourceMat = sourceUrpMats.materials[i];

            copyMaterialProps(ref sourceMat, ref newBetterLitMats.materials[i]);

            //newMat.GetTexturePropertyNames(); // use this to get property names if need to add a new texture to copy
        }

        AssetDatabase.Refresh();
        Debug.Log("Finished Copying Tetures");
    }

    private void CopyMatPropertiesToFolder() {
        if (BaseMaterial == null) {
            Debug.LogWarning($"Base material is null.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(SourceFolder.Folder))) {
            Debug.LogWarning("Source Folder is not a valid folder.");
            return;
        }

        if (!AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(DestinationFolder.Folder))) {
            Debug.LogWarning("Destination Folder is not a valid folder.");
            return;
        }

        string baseMaterialPath = AssetDatabase.GetAssetPath(BaseMaterial.material);

        string sourceFolderPath = AssetDatabase.GetAssetPath(SourceFolder.Folder);
        string destinationFolderPath = AssetDatabase.GetAssetPath(DestinationFolder.Folder);
        string[] SourceMaterialGuids = AssetDatabase.FindAssets("t:Material", new[] { sourceFolderPath });


        for (var i = 0; i < SourceMaterialGuids.Length; i++) {
            string SourceMaterialPath = AssetDatabase.GUIDToAssetPath(SourceMaterialGuids[i]);
            string NewMatPathWithName = Path.Combine(destinationFolderPath,
                Path.GetFileName(SourceMaterialPath));
            if (!AssetDatabase.CopyAsset(baseMaterialPath, NewMatPathWithName)) {
                Debug.LogWarning($"Something went wrong with copying asset {NewMatPathWithName}");
                continue;
            }

            AssetDatabase.Refresh();
            Material sourceMaterial = AssetDatabase.LoadAssetAtPath<Material>(SourceMaterialPath);
            Material NewMaterial = AssetDatabase.LoadAssetAtPath<Material>(NewMatPathWithName);
            copyMaterialProps(ref sourceMaterial, ref NewMaterial);
            Debug.Log($"{NewMaterial.name} Copied.");
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
    }

    private void copyMaterialProps(ref Material source, ref Material New) {
        if (!source) {
            Debug.LogWarning($"Source material is null. Skipping.");
            return;
        }

        Texture sourceAlbedo = source.GetTexture("_BaseMap");
        Texture sourceNormal = source.GetTexture("_BumpMap");
        Color BaseColor = source.GetColor("_Color");
        if (!New) {
            Debug.LogWarning($"Target material at is null. Skipping.");
            return;
        }

        New.SetTexture("_AlbedoMap", sourceAlbedo);
        New.SetTexture("_NormalMap", sourceNormal);
        New.SetColor("_Tint", BaseColor);
        RenameMat(ref New, source.name);
        EditorUtility.SetDirty(New);
    }

    private void RenameMat(ref Material mat, string newName) {
        string newPath = AssetDatabase.GetAssetPath(mat);
        if (!string.IsNullOrEmpty(newPath)) {
            string result = AssetDatabase.RenameAsset(newPath, newName);
            if (!string.IsNullOrWhiteSpace(result)) {
                Debug.LogWarning("Unable to Rename: " + result);
            }

            AssetDatabase.SaveAssets();
        }
    }

    private void displayMats(ref MaterialListWrapper mats, string label) {
        GUILayout.BeginVertical();
        SerializedObject serializedObject = new SerializedObject(mats);
        SerializedProperty materialList = serializedObject.FindProperty("materials");

        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        serializedObject.Update();
        EditorGUILayout.PropertyField(materialList, new GUIContent("Materials"), true);
        serializedObject.ApplyModifiedProperties();
        GUILayout.EndVertical();
    }

    [Serializable]
    private class MaterialListWrapper : ScriptableObject
    {
        public Material[] materials;
    }

    [Serializable]
    private class MaterialWrapper : ScriptableObject
    {
        public Material material;
    }

    [Serializable]
    private class FolderAssetWrapper : ScriptableObject
    {
        public DefaultAsset Folder;
    }

    private enum ConversionTypeE
    {
        OneToOne,
        FolderCopy
    }
}