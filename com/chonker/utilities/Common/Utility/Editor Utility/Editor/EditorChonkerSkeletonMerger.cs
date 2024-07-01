using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorChonkerSkeletonMerger : EditorWindow
{
    private SkinnedMeshRenderer skeletonBase;
    private SkinnedMeshRenderer smrToMerge;
    [MenuItem("Chonker/SMR skeleton merger")]
    private static void showWindow() {
        EditorChonkerSkeletonMerger wnd = GetWindow<EditorChonkerSkeletonMerger>();
        wnd.titleContent = new GUIContent("Merge Skeletons");
    }

    private void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Base SMR");
        skeletonBase = (SkinnedMeshRenderer) EditorGUILayout.ObjectField(skeletonBase, typeof(SkinnedMeshRenderer));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("SMR to Merge");
        smrToMerge = (SkinnedMeshRenderer) EditorGUILayout.ObjectField(smrToMerge, typeof(SkinnedMeshRenderer));
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Merge")) {
            Dictionary<string, Transform> baseSkeletonBones = skeletonBase.bones.ToDictionary(skeletonBaseBone => skeletonBaseBone.name);
            Transform[] bones = new Transform[smrToMerge.bones.Length];
            for (var i = 0; i < smrToMerge.bones.Length; i++) {
                if (!baseSkeletonBones.TryGetValue(smrToMerge.bones[i].name, out bones[i])) {
                    Debug.Log("Unable to map bone " + smrToMerge.bones[i].name);
                }
            }

            smrToMerge.bones = bones;

            smrToMerge.rootBone = baseSkeletonBones[smrToMerge.rootBone.name];
        }
        EditorGUILayout.HelpBox("This is to merge the skeleton of some model with another base model, without merging the the SMR's ", MessageType.Info);
    }

    private SkeletonBone findSkeletonBone(HumanBone humanBone, HumanDescription humanDescription) {
        foreach (var skeletonBone in humanDescription.skeleton) {
            if (skeletonBone.name == humanBone.boneName) {
                return skeletonBone;
            }
        }

        throw new Exception();
    }
}
