using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EditorBetterLitShaderConverter : EditorWindow
{
    [MenuItem("Chonker/Better Lit Shader Converter")]
    public static void ShowWindow() {
        EditorBetterLitShaderConverter windows = GetWindow<EditorBetterLitShaderConverter>();
        windows.titleContent = new GUIContent("Better Lit Shader Converter");
    }

    private MaterialListWrapper sourceUrpMats;
    private MaterialListWrapper newBetterLitMats;

    private void OnEnable() {
        // Create a transient instance of MaterialListWrapper
        sourceUrpMats = ScriptableObject.CreateInstance<MaterialListWrapper>();
        newBetterLitMats = ScriptableObject.CreateInstance<MaterialListWrapper>();
    }

    private void OnGUI() {
        GUILayout.BeginHorizontal();
        displayMats(ref sourceUrpMats, "Source Materials");
        displayMats(ref newBetterLitMats, "New Materials");

        GUILayout.EndHorizontal();

        EditorGUILayout.HelpBox("Material Lists must match One to One and must be in the correct order",
            MessageType.Info);

        if (GUILayout.Button("Copy Textures")) {
            if (sourceUrpMats.materials.Length != newBetterLitMats.materials.Length) {
                Debug.LogError("Material Lists must be the same length");
                return;
            }

            for (var i = 0; i < sourceUrpMats.materials.Length; i++) {
                Material sourceMat = sourceUrpMats.materials[i];
                if (!sourceMat) {
                    Debug.LogWarning($"Source material at index {i} is null. Skipping.");
                    continue;
                }

                Texture sourceAlbedo = sourceMat.GetTexture("_BaseMap");
                Texture sourceNormal = sourceMat.GetTexture("_BumpMap");
                Material newMat = newBetterLitMats.materials[i];
                if (!newMat) {
                    Debug.LogWarning($"Target material at index {i} is null. Skipping.");
                    continue;
                }

                newMat.SetTexture("_AlbedoMap", sourceAlbedo);
                newMat.SetTexture("_NormalMap", sourceNormal);
                //newMat.GetTexturePropertyNames(); // use this to get property names if need to add a new texture to copy
                EditorUtility.SetDirty(newMat);
            }


            Debug.Log("Finished Copying Tetures");
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

    [System.Serializable]
    private class MaterialListWrapper : ScriptableObject
    {
        public Material[] materials;
    }
}