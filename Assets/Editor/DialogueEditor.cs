using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogueEditorWindow))]

public class DialogueEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        {
            if (GUILayout.Button("Create Dialogue"))
            {
                Debug.Log("Created dialogue");
            }
        }
    }
}
