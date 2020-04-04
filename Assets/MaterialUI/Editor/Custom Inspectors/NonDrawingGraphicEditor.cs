using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(NonDrawingGraphic), false)]
public class NonDrawingGraphicEditor : GraphicEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        RaycastControlsGUI();
        serializedObject.ApplyModifiedProperties();
    }
}