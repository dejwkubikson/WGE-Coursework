using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow
{
    static DialogueEditorWindow window;

    [MenuItem("CustomWindows/DialogueWindow")]
    static void Init()
    {
        window = (DialogueEditorWindow)EditorWindow.GetWindow(typeof(DialogueEditorWindow));
        window.Show();
    }

    private void OnGUI()
    {
        using (var horizontalScope = new EditorGUILayout.HorizontalScope())
        {

        }

        // When create dialogue button is pressed
        if (GUILayout.Button("Create Dialogue"))
        {
            Debug.Log("Create dialogue");
        }
    }

}
