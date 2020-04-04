//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MaterialUI
{
    [CustomEditor(typeof(PanelExpander), true)]
    [CanEditMultipleObjects]
    public class PanelExpanderEditor : MaterialBaseEditor
    {
        private PanelExpander m_SelectedPanelExpander;
        private TargetArray<PanelExpander> m_SelectedPanelExpanders;

        private SerializedProperty m_ExpandMode;
        private SerializedProperty m_AutoExpandedSize;
        private SerializedProperty m_ExpandedSize;
        private SerializedProperty m_AutoExpandedPosition;
        private SerializedProperty m_ExpandedPosition;

        private SerializedProperty m_RippleHasShadow;
        private SerializedProperty m_AnimationDuration;
        private SerializedProperty m_DarkenBackground;
        private SerializedProperty m_ClickBackgroundToClose;

        private SerializedProperty m_PanelRootRectTransform;
        private SerializedProperty m_PanelContentCanvasGroup;
        private SerializedProperty m_PanelShadowCanvasGroup;
        private SerializedProperty m_BaseRectTransform;

        void OnEnable()
        {
            OnBaseEnable();

            m_SelectedPanelExpander = (PanelExpander)target;
            m_SelectedPanelExpanders = new TargetArray<PanelExpander>(targets);

            m_ExpandMode = serializedObject.FindProperty("m_ExpandMode");
            m_AutoExpandedSize = serializedObject.FindProperty("m_AutoExpandedSize");
            m_ExpandedSize = serializedObject.FindProperty("m_ExpandedSize");
            m_AutoExpandedPosition = serializedObject.FindProperty("m_AutoExpandedPosition");
            m_ExpandedPosition = serializedObject.FindProperty("m_ExpandedPosition");

            m_RippleHasShadow = serializedObject.FindProperty("m_RippleHasShadow");
            m_AnimationDuration = serializedObject.FindProperty("m_AnimationDuration");
            m_DarkenBackground = serializedObject.FindProperty("m_DarkenBackground");
            m_ClickBackgroundToClose = serializedObject.FindProperty("m_ClickBackgroundToClose");

            m_PanelRootRectTransform = serializedObject.FindProperty("m_PanelRootRectTransform");
            m_PanelContentCanvasGroup = serializedObject.FindProperty("m_PanelContentCanvasGroup");
            m_PanelShadowCanvasGroup = serializedObject.FindProperty("m_PanelShadowCanvasGroup");
            m_BaseRectTransform = serializedObject.FindProperty("m_BaseRectTransform");
        }

        void OnDisable()
        {
            OnBaseDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ExpandMode);
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.expandMode = (PanelExpander.ExpandMode)m_ExpandMode.enumValueIndex);
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_AutoExpandedSize, new GUIContent("Expanded Size"), GUILayout.ExpandWidth(false));
                if (EditorGUI.EndChangeCheck())
                {
                    m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.autoExpandedSize = m_AutoExpandedSize.boolValue);
                }

                using (new EditorGUI.DisabledScope(!m_AutoExpandedSize.boolValue))
                {
                    EditorGUILayout.LabelField("(Auto)", GUILayout.MaxWidth(40f));
                }

                using (new EditorGUI.DisabledScope(m_AutoExpandedSize.boolValue))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_ExpandedSize, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.expandedSize = m_ExpandedSize.vector2Value);
                    }
                }
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_AutoExpandedPosition, new GUIContent("Expanded Position"), GUILayout.ExpandWidth(false));
                if (EditorGUI.EndChangeCheck())
                {
                    m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.autoExpandedPosition = m_AutoExpandedPosition.boolValue);
                }

                using (new EditorGUI.DisabledScope(!m_AutoExpandedPosition.boolValue))
                {
                    EditorGUILayout.LabelField("(Auto)", GUILayout.MaxWidth(40f));
                }

                using (new EditorGUI.DisabledScope(m_AutoExpandedPosition.boolValue))
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(m_ExpandedPosition, GUIContent.none);
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.expandedPosition = m_ExpandedPosition.vector2Value);
                    }
                }
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_DarkenBackground);
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.darkenBackground = m_DarkenBackground.boolValue);
            }

            GUILayout.FlexibleSpace();

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_ClickBackgroundToClose, GUILayout.ExpandWidth(true));
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.clickBackgroundToClose = m_ClickBackgroundToClose.boolValue);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_RippleHasShadow);
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.rippleHasShadow = m_RippleHasShadow.boolValue);
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(m_AnimationDuration);
            if (EditorGUI.EndChangeCheck())
            {
                m_SelectedPanelExpanders.ExecuteAction(panelExpander => panelExpander.animationDuration = m_AnimationDuration.floatValue);
            }

            DrawFoldoutComponents(ComponentsSection);

            serializedObject.ApplyModifiedProperties();
        }

        private void ComponentsSection()
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(m_PanelRootRectTransform);
            EditorGUILayout.PropertyField(m_PanelContentCanvasGroup);
            EditorGUILayout.PropertyField(m_PanelShadowCanvasGroup);
            EditorGUILayout.PropertyField(m_BaseRectTransform);
            EditorGUI.indentLevel--;
        }
    }
}