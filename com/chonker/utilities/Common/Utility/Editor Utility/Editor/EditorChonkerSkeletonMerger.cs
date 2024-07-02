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
        skeletonBase = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(skeletonBase, typeof(SkinnedMeshRenderer));
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("SMR to Merge");
        smrToMerge = (SkinnedMeshRenderer)EditorGUILayout.ObjectField(smrToMerge, typeof(SkinnedMeshRenderer));
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Merge")) {
            Transform rootBone = skeletonBase.bones[0];
            Dictionary<string, Transform> baseSkeletonBones = rootBone.GetComponentsInChildren<Transform>()
                .ToDictionary(skeletonBaseBone => skeletonBaseBone.name);
            Transform[] bones = new Transform[smrToMerge.bones.Length];
            for (var i = 0; i < smrToMerge.bones.Length; i++) {
                Transform boneToMerge = smrToMerge.bones[i];
                if (!baseSkeletonBones.TryGetValue(boneToMerge.name, out bones[i])) {
                    Debug.Log($"Bone not found in base skeleton: {boneToMerge.name}... Attempting to add it");
                    if (baseSkeletonBones.TryGetValue(boneToMerge.parent.name, out Transform parentBone)) {
                        Transform additionalTargetBone =
                            parentBone.transform.Find(boneToMerge.name); // was the bone already added to the skeleton?
                        if (!additionalTargetBone) {
                            additionalTargetBone = copyBoneOver(parentBone, boneToMerge);
                        }

                        if (!baseSkeletonBones.ContainsKey(additionalTargetBone.name)) {
                            baseSkeletonBones.Add(additionalTargetBone.name, additionalTargetBone);
                        }

                        Debug.Log($"Added bone: {boneToMerge.name}");
                        bones[i] = additionalTargetBone;
                    }
                    else {
                        // bone not found
                        Debug.Log(
                            $"[SkinnedMeshRenderer({smrToMerge.name})] Unable to map bone [{boneToMerge.name}] to target skeleton.");
                    }
                }
            }

            smrToMerge.bones = bones;

            smrToMerge.rootBone = baseSkeletonBones[smrToMerge.rootBone.name];
        }

        EditorGUILayout.HelpBox(
            "This is to merge the skeleton of some model with another base model, without merging the the SMR's ",
            MessageType.Info);
    }

    private Transform copyBoneOver(Transform baseModelParentBone, Transform mergeModelBone) {
        string mergeModelBoneName = mergeModelBone.name;
        Transform copiedBone = new GameObject(mergeModelBoneName).transform;
        copiedBone.parent = baseModelParentBone;
        copiedBone.localPosition = mergeModelBone.localPosition;
        copiedBone.localRotation = mergeModelBone.localRotation;
        copiedBone.localScale = mergeModelBone.localScale;
        return copiedBone;
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