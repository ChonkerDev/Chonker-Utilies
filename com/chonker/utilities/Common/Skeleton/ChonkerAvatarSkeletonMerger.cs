using System.Collections.Generic;
using System.Linq;

#if MAGICACLOTH2
using MagicaCloth2;
#endif
using UnityEngine;

/*
 * core logic borrowed from MagicaSoft, but added additional functionality if there are extra bones
 */


public class ChonkerAvatarSkeletonMerger : MonoBehaviour
{
    /// <summary>
    /// Avatar to change clothes.
    /// </summary>
    private GameObject targetAvatar;

    //=========================================================================================
    /// <summary>
    /// Bones dictionary of avatars to dress up.
    /// </summary>
    Dictionary<string, Transform> targetAvatarBoneMap = new Dictionary<string, Transform>();

    //=========================================================================================
    private void Awake() {
        Init();
    }


    //=========================================================================================
    //=========================================================================================
    /// <summary>
    /// Create an avatar bone dictionary in advance.
    /// </summary>
    void Init() {
        targetAvatar = gameObject;
        Debug.Assert(targetAvatar);

        // Create all bone maps for the target avatar
        foreach (Transform bone in targetAvatar.GetComponentsInChildren<Transform>()) {
            if (targetAvatarBoneMap.ContainsKey(bone.name) == false) {
                targetAvatarBoneMap.Add(bone.name, bone);
            }
            else {
                Debug.Log($"Duplicate bone name :{bone.name}");
            }
        }
    }

    /// <summary>
    /// Equip clothes.
    /// </summary>
    /// <param name="equipPrefab"></param>
    /// <param name="einfo"></param>
    public void Equip(GameObject equipPrefab) {
        Debug.Assert(equipPrefab);

        // Generate a prefab with cloth set up.
        var gobj = Instantiate(equipPrefab, targetAvatar.transform);

#if MAGICACLOTH2
            // All cloth components included in the prefab.
            var clothList = new List<MagicaCloth>(gobj.GetComponentsInChildren<MagicaCloth>());

            // All collider components included in the prefab.
            var colliderList = new List<ColliderComponent>(gobj.GetComponentsInChildren<ColliderComponent>());
#endif

        //　All renderers included in the prefab.
        var skinList = new List<SkinnedMeshRenderer>(gobj.GetComponentsInChildren<SkinnedMeshRenderer>());

#if MAGICACLOTH2
            // First stop the automatic build that is executed with Start().
            // And just in case, it does some initialization called Awake().
            foreach (var cloth in clothList) {
                // Normally it is called with Awake(), but if the component is disabled, it will not be executed, so call it manually.
                // Ignored if already run with Awake().
                cloth.Initialize();

                // Turn off auto-build on Start().
                cloth.DisableAutoBuild();
            }
#endif

        // Swap the bones of the SkinnedMeshRenderer.
        // This process is a general dress-up process for SkinnedMeshRenderer.
        // Comment out this series of processes when performing this process with functions such as other assets.
        foreach (var sren in skinList) {
            var bones = sren.bones;
            Transform[] newBones = new Transform[bones.Length];

            for (int i = 0; i < bones.Length; ++i) {
                Transform bone = bones[i];
                if (!targetAvatarBoneMap.TryGetValue(bone.name, out var foundBone)) {
                    // Is the bone the renderer itself?
                    if (bone.name == sren.name) {
                        newBones[i] = sren.transform;
                    }
                    else {
                        // is the bone part of the skeleton but not in the original? for situations where a skinned mesh renderer contains an additional bone
                        if (targetAvatarBoneMap.TryGetValue(bone.parent.name, out Transform parentBone)) {
                            Transform additionalTargetBone =
                                parentBone.transform.Find(bone.name); // was the bone already added to the skeleton?
                            if (!additionalTargetBone) {
                                Quaternion oldLocalRotation = bone.transform.localRotation;
                                Vector3 oldLocalPosition = bone.transform.localPosition;
                                bone.transform.parent = parentBone;
                                bone.transform.localRotation = oldLocalRotation;
                                bone.transform.localPosition = oldLocalPosition;
                                additionalTargetBone = bone;
                            }

                            if (!targetAvatarBoneMap.ContainsKey(additionalTargetBone.name)) {
                                targetAvatarBoneMap.Add(additionalTargetBone.name, additionalTargetBone);
                            }

                            newBones[i] = additionalTargetBone;
                        }
                        else {
                            // bone not found
                            Debug.Log(
                                $"[SkinnedMeshRenderer({sren.name})] Unable to map bone [{bone.name}] to target skeleton.");
                        }
                    }
                }
                else {
                    newBones[i] = foundBone;
                }
            }

            sren.bones = newBones.ToArray();

            // root bone
            if (targetAvatarBoneMap.ContainsKey(sren.rootBone?.name)) {
                sren.rootBone = targetAvatarBoneMap[sren.rootBone.name];
            }
        }

#if MAGICACLOTH2
            // Here, replace the bones used by the MagicaCloth component.
            foreach (var cloth in clothList) {
                // Replaces a component's transform.
                cloth.ReplaceTransform(targetAvatarBoneMap);
            }

            // Move all colliders to the new avatar.
            foreach (var collider in colliderList) {
                Transform parent = collider.transform.parent;
                if (parent && targetAvatarBoneMap.ContainsKey(parent.name)) {
                    Transform newParent = targetAvatarBoneMap[parent.name];

                    // After changing the parent, you need to write back the local posture and align it.
                    var localPosition = collider.transform.localPosition;
                    var localRotation = collider.transform.localRotation;
                    collider.transform.SetParent(newParent);
                    collider.transform.localPosition = localPosition;
                    collider.transform.localRotation = localRotation;
                }
            }

            // Finally let's start building the cloth component.
            foreach (var cloth in clothList) {
                // I disabled the automatic build, so I build it manually.
                cloth.BuildAndRun();
            }
#endif
    }
}