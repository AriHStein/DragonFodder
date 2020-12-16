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
        //base.OnInspectorGUI();

        ////SerializedProperty unitProperty = serializedObject.FindProperty("Units");
        //if (m_units != null)
        //{
        //    m_size = new Vector2Int(m_units.arraySize, m_units.GetArrayElementAtIndex(0).arraySize);
        //}
        ////if (squad.Units != null && squad.Units[0] != null)
        ////{
        ////    m_size = new Vector2Int(squad.Units.Length, squad.Units[0].Length);
        ////}
        //else
        //{
        //    m_size = Vector2Int.one;
        //}

        //m_size.vector2IntValue = EditorGUILayout.Vector2IntField("Size", m_size.vector2IntValue);
        //SquadPrototype squad = (SquadPrototype)target;
        //m_size.Resize(m_size);
        //serializedObject.ApplyModifiedProperties();

        //serializedObject.Update();
        //m_units = serializedObject.FindProperty("Units");
        //if (m_units == null || m_units.GetArrayElementAtIndex(0) == null)
        //{
        //    serializedObject.ApplyModifiedProperties();
        //    return;
        //}

        for (int y = 0; y < m_size.vector2IntValue.y; y++)
        {

            GUILayout.BeginHorizontal();
            for(int x = 0; x < m_size.vector2IntValue.x; x++)
            {
                SerializedProperty unitArray = m_units.GetArrayElementAtIndex(x).FindPropertyRelative("Items");
                //if(unitArray == null)
                //{
                //    Debug.Log("UnitArray is null.");
                //    return;
                //}

                unitArray.GetArrayElementAtIndex(y).objectReferenceValue = (UnitPrototype)EditorGUILayout.ObjectField(
                    unitArray.GetArrayElementAtIndex(y).objectReferenceValue, 
                    typeof(UnitPrototype), 
                    false, GUILayout.MinWidth(30), GUILayout.MinHeight(30));
            }

            GUILayout.EndHorizontal();

        }

        serializedObject.ApplyModifiedProperties();
        //AssetDatabase.Refresh();
        //EditorUtility.SetDirty(squad);
        //AssetDatabase.SaveAssets();
    }
}
