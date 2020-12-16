using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SquadPrototype))]
public class SquadPrototypeEditor : Editor
{
    SerializedProperty m_units;
    SerializedProperty m_size;

    private void OnEnable()
    {
        m_units = serializedObject.FindProperty("Units");
        if(m_units == null)
        {
            Debug.Log("Units not found.");
        }
        m_size = serializedObject.FindProperty("Size");
        if (m_size == null)
        {
            Debug.Log("Units not found.");
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        if (m_units == null)
        {
            m_units = serializedObject.FindProperty("Units");
            if (m_units == null)
            {
                Debug.Log("Units still not found.");
                return;
            }
        }
        DrawDefaultInspector();

        for (int y = 0; y < m_size.vector2IntValue.y; y++)
        {

            GUILayout.BeginHorizontal();
            for(int x = 0; x < m_size.vector2IntValue.x; x++)
            {
                SerializedProperty unitArray = m_units.GetArrayElementAtIndex(x).FindPropertyRelative("Items");

                unitArray.GetArrayElementAtIndex(y).objectReferenceValue = (UnitPrototype)EditorGUILayout.ObjectField(
                    unitArray.GetArrayElementAtIndex(y).objectReferenceValue, typeof(UnitPrototype), 
                    false, GUILayout.MinWidth(30), GUILayout.MinHeight(30));
            }

            GUILayout.EndHorizontal();

        }

        serializedObject.ApplyModifiedProperties();
    }
}
