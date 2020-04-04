//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
using System.Linq;

namespace MaterialUI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(VectorImage))]
    class VectorImageEditor : MaterialBaseEditor
    {
        //  SerializedProperties
        private SerializedProperty m_Size;
        private SerializedProperty m_SizeMode;

        void OnEnable()
        {
            OnBaseEnable();
            m_Size = serializedObject.FindProperty("m_Size");
            m_SizeMode = serializedObject.FindProperty("m_SizeMode");
        }

        void OnDisable()
        {
            OnBaseDisable();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(m_SizeMode);
            if (m_SizeMode.enumValueIndex == 0)
            {
                EditorGUILayout.PropertyField(m_Size);
            }

            InspectorFields.GraphicColorMultiField("Icon", gameObject => gameObject.GetComponent<VectorImage>());

            serializedObject.ApplyModifiedProperties();
        }
    }
}