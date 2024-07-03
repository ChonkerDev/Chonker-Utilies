using System;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
public class ChonkerEditorTPoseForcer : EditorWindow
{
    private Animator rootAnimator;
    [MenuItem("Chonker/TPose Forcer")]
    private static void showWindow() {
        ChonkerEditorTPoseForcer wnd = GetWindow<ChonkerEditorTPoseForcer>();
        wnd.titleContent = new GUIContent("Force T Pose");
    }

    private void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Root Animator");
        rootAnimator = (Animator) EditorGUILayout.ObjectField(rootAnimator, typeof(Animator));
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reset Character To T Pose")) {
            for (var i = 0; i < rootAnimator.avatar.humanDescription.human.Length; i++) {
                HumanBone humanBone = rootAnimator.avatar.humanDescription.human[i];
                HumanBodyBones humanBoneToFind = Enum.Parse<HumanBodyBones>(humanBone.humanName.Replace(" ", ""));
                SkeletonBone skeletonBone = findSkeletonBone(humanBone, rootAnimator.avatar.humanDescription);
                Vector3 avatarBonePosition = skeletonBone.position;
                Quaternion avatarBoneRotation = skeletonBone.rotation;
                Vector3 avatarBoneScale = skeletonBone.scale;

                Transform bone = rootAnimator.GetBoneTransform(humanBoneToFind);
                bone.localPosition = avatarBonePosition;
                bone.localRotation = avatarBoneRotation;
                bone.localScale = avatarBoneScale;

            }
        }
        EditorGUILayout.HelpBox("Note: This will not effect runtime animations, this is just to reset to the default pose in the editor scene view", MessageType.Info);
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
#endif