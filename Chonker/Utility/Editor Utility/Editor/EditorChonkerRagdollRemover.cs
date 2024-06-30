using System.Linq;
using UnityEditor;
using UnityEngine;

public class EditorChonkerRagdollRemover : EditorWindow
{
    private static GameObject rootBone;

    [MenuItem("Chonker/Ragdoll Remover")]
    private static void showWindow() {
        EditorChonkerRagdollRemover wnd = GetWindow<EditorChonkerRagdollRemover>();
        wnd.titleContent = new GUIContent("Chonker Ragdoll Remover");
    }

    private void OnGUI() {
        rootBone = (GameObject) EditorGUILayout.ObjectField(rootBone, typeof(GameObject));

        if (GUILayout.Button("Removed Ragdoll Components")) {
            Rigidbody[] rbs = rootBone.transform.GetComponentsInChildren<Rigidbody>();
            
            for (var i = 0; i < rbs.Length; i++) {
                Rigidbody rb = rbs[i];
                if (rb.TryGetComponent(out CharacterJoint joint)) {
                    DestroyImmediate(joint);
                }

                if (rb.TryGetComponent(out Collider collider)) {
                    DestroyImmediate(collider);
                }
                
                DestroyImmediate(rb);
            }
                
            
        }
    }
}