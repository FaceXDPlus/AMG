//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;

namespace MaterialUI
{
    [CustomEditor(typeof(MaterialUIScaler))]
    public class MaterialUIScalerEditor : Editor
    {
        private Canvas m_CanvasObject;
        public Canvas canvasObject
        {
            get
            {
                if (m_CanvasObject == null)
                {
                    m_CanvasObject = ((MaterialUIScaler)target).canvas;
                }
                return m_CanvasObject;
            }
        }

        private MaterialUIScaler scaler;

        private SerializedProperty m_ScalerMode;
        private SerializedProperty m_EditorForceDpi;
        private SerializedProperty m_EditorForceDpiValue;
        private SerializedProperty m_ReferencePixelsPerUnit;
        private SerializedProperty m_DynamicPixelsPerUnit;
        private SerializedProperty m_BaseDpi;
        private SerializedProperty m_FallbackDpi;
        private SerializedProperty m_ScaleModifier;
        private SerializedProperty m_ScaleSnapLevel;

        private SerializedProperty m_TargetScaler;
        private SerializedProperty m_TargetCanvasScaler;

        void OnEnable()
        {
            scaler = target as MaterialUIScaler;

            m_ScalerMode = serializedObject.FindProperty("m_ScalerMode");
            m_EditorForceDpi = serializedObject.FindProperty("m_EditorForceDpi");
            m_EditorForceDpiValue = serializedObject.FindProperty("m_EditorForceDpiValue");
            m_ReferencePixelsPerUnit = serializedObject.FindProperty("m_ReferencePixelsPerUnit");
            m_DynamicPixelsPerUnit = serializedObject.FindProperty("m_DynamicPixelsPerUnit");
            m_BaseDpi = serializedObject.FindProperty("m_BaseDpi");
            m_FallbackDpi = serializedObject.FindProperty("m_FallbackDpi");
            m_ScaleModifier = serializedObject.FindProperty("m_ScaleModifier");
            m_ScaleSnapLevel = serializedObject.FindProperty("m_ScaleSnapLevel");

            m_TargetScaler = serializedObject.FindProperty("m_TargetScaler");
            m_TargetCanvasScaler = serializedObject.FindProperty("m_TargetCanvasScaler");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_ScalerMode);

            switch (m_ScalerMode.enumValueIndex)
            {
                case 0:
                    DrawPhysicalProperties();
                    break;
                case 1:
                    DrawCopyOtherScalerProperties();
                    break;
                case 2:
                    DrawDontChangeScaleProperties();
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawPhysicalProperties()
        {
            if (canvasObject.renderMode == RenderMode.WorldSpace)
            {
                EditorGUILayout.PropertyField(m_DynamicPixelsPerUnit);
            }
            else
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PropertyField(m_EditorForceDpi);
                    EditorGUILayout.PropertyField(m_EditorForceDpiValue, new GUIContent());
                }

                EditorGUILayout.PropertyField(m_BaseDpi);
                EditorGUILayout.PropertyField(m_FallbackDpi);
                EditorGUILayout.PropertyField(m_ScaleModifier);
                EditorGUILayout.IntSlider(m_ScaleSnapLevel, 0, 16);

                EditorGUILayout.PropertyField(m_ReferencePixelsPerUnit);

                EditorGUILayout.LabelField(scaler.screenWidth + " x " + scaler.screenHeight + ", " + scaler.dpi + " dpi, " + scaler.screenSizeDigonal.ToString("##.##") + " inches");
            }
        }

        private void DrawCopyOtherScalerProperties()
        {
            if (m_TargetScaler.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox("This scaler will not do anything without a reference to copy from",
                    MessageType.Warning);
            }

            EditorGUILayout.PropertyField(m_TargetScaler);
        }

        private void DrawDontChangeScaleProperties()
        {
            if (m_TargetCanvasScaler.objectReferenceValue != null)
            {
                EditorGUILayout.HelpBox(
                    "CanvasScaler attached. It will be used to invoke scale/orientation change callbacks.",
                    MessageType.Info);
            }
        }
    }
}