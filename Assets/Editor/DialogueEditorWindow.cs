using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow
{
    static DialogueEditorWindow window;

    public string dialogueFileNameToSave;
    public string dialogueFileNameToLoad;
    public string speakerName;


    // make lists
    public string speakerText;
    public string speakerKey;

    //public Dictionary<string, string> speakerDict;

    // make lists
    public string playerText;
    //public string playerKey;

    //public Dictionary<string, string> playerDict;

    // make list
    bool endBtn = false;
    
    private int conversationCounter = 0;
    private int popUpIndex;

    public Dictionary<string, string> speakerDict = new Dictionary<string, string>();
    public Dictionary<string, string> playerDict = new Dictionary<string, string>();
    public Dictionary<string, bool> playerTextEndsDialogue = new Dictionary<string, bool>();
    public Dictionary<int, int> conversationDict = new Dictionary<int, int>();
    public Dictionary<int, int> conversationOptionDict = new Dictionary<int, int>();

    Vector2 scrollPos = Vector2.zero;
    private bool allowToAddOption = true;

    [MenuItem("CustomWindows/DialogueWindow")]
    static void Init()
    {
        window = (DialogueEditorWindow)EditorWindow.GetWindow(typeof(DialogueEditorWindow));
        window.Show();
    }

    private void AddConversation()
    {
        //Debug.Log("Called AddConversation(), conversationCounter is " + conversationCounter);

        conversationDict.Add(conversationCounter, 0);
        conversationCounter++;
    }

    private void RemoveConversation()
    {
        // If there was at least one conversation added
        if(conversationCounter > 0)
            conversationDict.Remove(conversationCounter - 1);
    }

    private void ClearConversations()
    {
        conversationCounter = 0;

        speakerDict.Clear();
        playerDict.Clear();
        playerTextEndsDialogue.Clear();
        conversationDict.Clear();
        conversationOptionDict.Clear();
    }

    private void AddOption(int whichKey)
    {
        //Debug.Log("Called AddOption() with key " + whichKey);

        conversationDict[whichKey] += 1;

    }

    private void RemoveOption(int whichKey)
    {
        //Debug.Log("Called RemoveOption() with key " + whichKey);

        conversationDict[whichKey] -= 1;

        foreach (string key in playerDict.Keys)
        {
            if(key == "option" + (whichKey+1))
            {
                playerDict.Remove(key);
                // Stopping foreach loop so that the dictionary doesn't go out of sync.
                break;
            }
        }
    }

    private void LoadDialogueToInspector()
    {

    }

    private void LoadDialogueToScene()
    {
        DialogueScript dialogueScript;
        dialogueScript = GameObject.Find("DialogueObject").GetComponent<DialogueScript>();
        dialogueScript.dialogueFileName = dialogueFileNameToLoad;
    }

    // Runs every frame
    private void OnGUI()
    {
        // Used for scrolling
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos, true, true, GUILayout.Height(Screen.height));

        // Showing where the dialogue starts
        EditorGUILayout.LabelField("Start of the dialogue");
        // The name of the file that is supposed to be created
        dialogueFileNameToSave = EditorGUILayout.TextField("Name of file to create: ", dialogueFileNameToSave);
        // Speaker's name (NPC name)
        speakerName = EditorGUILayout.TextField("Speaker (NPC) name: ", speakerName);

        // Created to add space and make it more clear for the user
        EditorGUILayout.LabelField("");

        // Storing player texts in a new array, first element is constant as it will be a default element in select lists
        string[] playerValues = new string[playerDict.Count + 1];// { "I'm doing okay.", "I'm doing terribly.", "I'm doing Great!" }; // placeholder
        playerValues[0] = "Please select one of the options";

        // Starting the valuesIterator from 1 as it will be the index for the playerValues array
        int valuesIterator = 1;

        // Using keys as checking if the option should be displayed - if the text ends the dialogue the user shouldn't be able to use it 
        foreach (string key in playerDict.Keys)
        {
            if (!(playerTextEndsDialogue[key]))
                playerValues[valuesIterator] = playerDict[key];
            valuesIterator++;
        }

        // Creating a dictionary iterator which will be used in functions as a key of conversationDict dictionary
        int dictIterator = 0;
        // For each conversation displaying text fields and options accociated to that conversation
        foreach (int key in conversationDict.Keys)
        {
            if (dictIterator == 0)
            {
                // If it's the first conversation the user is able to add options to this conversation
                allowToAddOption = true;

                // Checking if 'start' key exists
                if (!(speakerDict.ContainsKey("start")))
                    speakerDict.Add("start", "");

                // What the speaker (NPC) is supposed to say at the start of this conversation
                speakerText = EditorGUILayout.TextField("Speaker (NPC) start text: ", speakerDict["start"]);
                speakerDict["start"] = speakerText;

                // Clearing speakerText after adding it to the dictionary
                speakerText = "";
            }
            else
            {
                // Checking if the key already exists
                if (!(conversationOptionDict.ContainsKey(key)))
                    conversationOptionDict.Add(key, 0);

                EditorGUILayout.LabelField("What does the player say to start this conversation?");
                
                // Prevents of creating new keys when the user chooses one option first and changes it. In order to do that he will need to remove the conversation 
                //if(conversationOptionDict[key])

                // The user needs to select what player's text started this conversation
                popUpIndex = EditorGUILayout.Popup(conversationOptionDict[key], playerValues);

                // Assigning the value to the dictionary which will then create appropriate ID for the player's option
                conversationOptionDict[key] = popUpIndex;
                
                // If the select list is in it's default position it won't allow the user to add any options to this conversation
                if (popUpIndex == 0)
                    allowToAddOption = false;
                else
                    allowToAddOption = true;

                speakerText = EditorGUILayout.TextField("Speaker (NPC) text: ", speakerText);
            }

            // Displaying options the player will be able to choose from
            for (int i = 0; i < conversationDict[key]; i++)
            {
                // Used to make the code a little bit more clear
                string optionID = "option";
                // If it's the first conversation options should be layed out like this: option1, option2, option3...
                if (dictIterator == 0)
                {
                    optionID += (i + 1);

                    // If the dictionary doesn't contain this key
                    if (!(playerDict.ContainsKey(optionID)))
                    {
                        playerDict.Add(optionID, "");
                        playerTextEndsDialogue.Add(optionID, false);
                    }
                }
                else
                {
                    // If it's another conversation (2nd, 3rd...) options are layed out like this: 
                    // option1.1, option1.2 or option2.1, option2.2 or option2.1.1, option2.1.2 and so on
                    // depending on which player's text leads here
                    //optionID = "option" + conversationDict[key] + "." + (i + 1);

                    // used for adding keys
                    foreach (string playerKey in playerDict.Keys)
                    {
                        if (playerDict[playerKey].Contains(playerValues[popUpIndex]))
                        {
                            optionID = playerKey + "." + conversationDict[key];
                        }
                    }
                    
                    // If the dictionary doesn't contain this key
                    if (!(playerDict.ContainsKey(optionID)))
                    {
                        playerDict.Add(optionID, "");
                        playerTextEndsDialogue.Add(optionID, false);
                    }

                    Debug.Log("key = " + key + " value = " + conversationDict[key] + " option key = " + key + " value = " + conversationOptionDict[key] + " creates " + optionID);
                }

                GUILayout.BeginHorizontal();
                playerText = EditorGUILayout.TextField("Player text: ", playerDict[optionID]);
                endBtn = EditorGUILayout.Toggle("Ends dialogue?", playerTextEndsDialogue[optionID]);
                GUILayout.EndHorizontal();

                // Assigning value to the dictionaries
                playerDict[optionID] = playerText;
                playerTextEndsDialogue[optionID] = endBtn;

                // Clearing playerText after adding it to the dictionary
                playerText = "";
            }

            dictIterator++;

            // If the user presses a button to add option
            if (GUILayout.Button("Add option to conversation " + (dictIterator)))
            {
                // The user can add option only if he selected which player's text starts this conversation
                if(allowToAddOption)
                    AddOption(dictIterator - 1);
                else
                    EditorUtility.DisplayDialog("Select text first!", "Before adding any options please select text that brings the player to this conversation.", "Ok");
                // Stopping the foreach loop as the dictionary could go out of sync. Therefore the dictionary won't change while looping which could end up in an infinite loop.
                break;
            }

            // If conversation has at least 1 option adding a 'Remove Option' button
            if (conversationDict[key] > 0)
            {
                if (GUILayout.Button("Remove Option"))
                {
                    RemoveOption(dictIterator - 1);
                    // Stopping the foreach loop as the dictionary could go out of sync. Therefore the dictionary won't change while looping which could end up in an infinite loop.
                    break;
                }
            }
            // Created to add space between each conversation, makes it clearer for the user which conversation he is at
            EditorGUILayout.LabelField("");
        }

        // If add conversation button was pressed
        if (GUILayout.Button("Add conversation"))
        {
            AddConversation();
        }

        // If remove conversation button was pressed
        if (GUILayout.Button("Remove conversation"))
        {
            if (EditorUtility.DisplayDialog("Remove conversation?", "Are you sure you want to remove last conversation?", "Remove", "Cancel"))
                RemoveConversation();
        }

        // Created to add space, reducing the error of pressing accidentaly to clear all conversations
        EditorGUILayout.LabelField("");
        // If clear conversations button was pressed
        if (GUILayout.Button("Clear conversations"))
        {
            if(EditorUtility.DisplayDialog("Clear all conversations?", "Are you sure you want to clear all conversations?", "Clear all", "Cancel"))
                ClearConversations();
        }

        // Showing the user where the dialogue edition ends
        EditorGUILayout.LabelField("End of the dialogue");

        // When create dialogue button is pressed
        if (GUILayout.Button("Create dialogue"))
        {
            Debug.Log("speaker " + speakerName);
        }

        // Created to add space to make it more clear that this section doesn't belong to the dialogue editor
        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Load dialogue from file");
        dialogueFileNameToLoad = EditorGUILayout.TextField("Name of file to load: ", dialogueFileNameToLoad);

        // If the user wants to load the dialogue to the inspector
        if (GUILayout.Button("Load dialogue to inspector"))
        {
            LoadDialogueToInspector();
        }

        // If the user wants to load the dialogue to the scene
        if (GUILayout.Button("Load dialogue to scene"))
        {
            LoadDialogueToScene();
        }

        // Created to add space
        EditorGUILayout.LabelField("");
        EditorGUILayout.EndScrollView();

        /*foreach(string key in playerDict.Keys)
        {
            if(playerTextEndsDialogue[key])
                Debug.Log("Player key " + key + " with text " + playerDict[key] + " ends the dialogue");
            else
                Debug.Log("Player key " + key + " with text " + playerDict[key] + " doesn't end the dialogue");
        }*/
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}

class converstion
{
    string id;
    string text;
    //List of options;
}

class options
{
    string text;
    string next;
}