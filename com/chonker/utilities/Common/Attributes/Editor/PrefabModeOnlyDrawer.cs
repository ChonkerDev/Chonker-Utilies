#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using UnityEngine;
#if UNITY_EDITOR
[CustomPropertyDrawer( typeof( PrefabModeOnlyAttribute ) )]
#endif
public class PrefabModeOnlyDrawer 
    #if UNITY_EDITOR
    : PropertyDrawer
#endif
{
#if UNITY_EDITOR

    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        return ( PrefabStageUtility.GetCurrentPrefabStage() == null ) ? 0 : EditorGUI.GetPropertyHeight( property, label );
    }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        if ( PrefabStageUtility.GetCurrentPrefabStage() == null ) return;
        EditorGUI.PropertyField( position, property, label );
    }
    #endif
}
