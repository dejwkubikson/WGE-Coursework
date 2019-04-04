using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow
{
    static DialogueEditorWindow window;

    public string speaker;
    public string dialogueFileName;
    public string speakerText;
    public string speakerKey;
    public Dictionary<string, string> speakerDict;
    public string playerText;
    public string playerKey;
    public Dictionary<string, string> playerDict;
    bool endBtn = true;

    private int conversationNumber = 0;
    private int popUpIndex;

    [MenuItem("CustomWindows/DialogueWindow")]
    static void Init()
    {
        window = (DialogueEditorWindow)EditorWindow.GetWindow(typeof(DialogueEditorWindow));
        window.Show();
    }

    private void AddConversation()
    {
        Debug.Log("Called AddConversation()");

        speakerText = EditorGUILayout.TextField("Speaker text: ", speakerText);

        if (conversationNumber == 0)
        {
            speakerDict.Add("start", speakerText);
        }
        else
        {
            string[] playerValues = new string[playerDict.Count];
            int index = 0;
            foreach(string value in playerDict.Values)
            {
                playerValues[index] = value;
                index++;
            }
            EditorGUILayout.Popup(index, playerValues);
        }
        
        conversationNumber++;
    }

    private void AddOption()
    {
        GUILayout.BeginHorizontal();
        playerText = EditorGUILayout.TextField("Player text: ", playerText);
        endBtn = EditorGUILayout.Toggle("Ends dialogue?", endBtn);
        GUILayout.EndHorizontal();
    }

    private void OnGUI()
    {
        speakerDict = new Dictionary<string, string>();
        playerDict = new Dictionary<string, string>();

        EditorGUILayout.LabelField("Start of the dialogue");
        dialogueFileName = EditorGUILayout.TextField("Name of file to create: ", dialogueFileName);
        speaker = EditorGUILayout.TextField("Speaker (NPC) name: ", speaker);

        if (GUILayout.Button("Add conversation"))
        {
            AddConversation();
        }

        // THIS NEEDS TO BE IN ADD CONVERSATION FUNCTION
        EditorGUILayout.LabelField("<conversation>");
        speakerText = EditorGUILayout.TextField("Speaker (NPC) text: ", speakerText);
        string[] playerValues = new string[] { "I'm doing okay.", "I'm doing terribly.", "I'm doing Great!" }; // placeholder
        EditorGUILayout.LabelField("Which text does the player say to start this conversation?");
        popUpIndex = EditorGUILayout.Popup(popUpIndex, playerValues);
        if (GUILayout.Button("Add option"))
        {
            AddOption();
        }
        // THIS NEEDS TO BE IN ADD OPTION FUNCTION
        EditorGUILayout.LabelField("<option>");
        GUILayout.BeginHorizontal();
        playerText = EditorGUILayout.TextField("Player text: ", playerText);
        endBtn = EditorGUILayout.Toggle("Ends dialogue?", endBtn);
        GUILayout.EndHorizontal();
        EditorGUILayout.LabelField("</option>");
        // THIS NEEDS TO BE IN ADD OPTION FUNCTION
        EditorGUILayout.LabelField("</conversation>");
        // THIS NEEDS TO BE IN ADD CONVERSATION FUNCTION

        EditorGUILayout.LabelField("End of the dialogue");
        // When create dialogue button is pressed
        if (GUILayout.Button("Create Dialogue"))
        {
            Debug.Log("speaker " + speaker);
        }
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}
