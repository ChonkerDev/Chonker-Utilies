using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomPropertyDrawer( typeof( PrefabModeOnlyAttribute ) )]
public class PrefabModeOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight( SerializedProperty property, GUIContent label )
    {
        return ( PrefabStageUtility.GetCurrentPrefabStage() == null ) ? 0 : EditorGUI.GetPropertyHeight( property, label );
    }
    public override void OnGUI( Rect position, SerializedProperty property, GUIContent label )
    {
        if ( PrefabStageUtility.GetCurrentPrefabStage() == null ) return;
        EditorGUI.PropertyField( position, property, label );
    }
}
