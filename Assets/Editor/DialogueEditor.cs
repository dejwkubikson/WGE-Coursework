using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Conversation
{
    public string conversationID;
    public string speakerText;
    public string speakerName;
    public int popupIndex; // used for the needs to show which option has been selected
    public List<Option> optionsList = new List<Option>();
}

public class Option
{
    public string playerText;
    public string nextConversationID;
    public bool endsDialogue;
}

public class DialogueEditor : EditorWindow
{
    static DialogueEditor window;

    List<Conversation> conversationList = new List<Conversation>();

    public string dialogueFileNameToSave;
    public string dialogueFileNameToLoad;
    public string speakerName;

    public string speakerText;
    public string speakerKey;

    public string playerText;

    // make list
    bool endBtn = false;

    private int conversationCounter = 0;
    private int popUpIndex;

    Vector2 scrollPos = Vector2.zero;
    private bool allowToAddOption = true;

    [MenuItem("CustomWindows/DialogueWindow")]
    static void Init()
    {
        window = (DialogueEditor)EditorWindow.GetWindow(typeof(DialogueEditor));
        window.Show();
    }

    private void AddConversation()
    {
        Conversation conversation = new Conversation();
        conversation.conversationID = "";
        conversation.speakerText = "";
        conversation.speakerName = this.speakerName;
        conversation.popupIndex = 0;
        conversationList.Add(conversation);
    }

    private void RemoveConversation(int whichConversation)
    {
        conversationList.RemoveAt(whichConversation);
    }

    private void ClearConversations()
    {
        conversationList.Clear();
    }

    private void AddOption(int whichConversation)
    {
        Option option = new Option();
        option.playerText = "";
        option.nextConversationID = "";
        option.endsDialogue = false;

        conversationList[whichConversation].optionsList.Add(option);
    }

    private void RemoveOption(int whichConversation)
    {
        // Removing last option from chosen conversation
        conversationList[whichConversation].optionsList.RemoveAt(conversationList[whichConversation].optionsList.Count - 1);
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

        // Getting the number of player options to create playerTexts array with right size - only options that don't end the dialogue are added
        int numberOfPlayerOptions = 0;
        foreach(Conversation conversation in conversationList)
        {
            foreach(Option option in conversation.optionsList)
            {
                if (!(option.endsDialogue))
                    numberOfPlayerOptions++;
            }

        }

        // Storing player texts in a new array, first element is constant as it will be a default element in select lists
        string[] playerTexts = new string[numberOfPlayerOptions + 2];
        playerTexts[0] = "Please select one of the options";

        // Starting the textIterator from 1 as it will be the index of the playerTexts array
        int textIterator = 1;

        // Adding the options the player can choose to playerTexts array
        foreach(Conversation conversation in conversationList)
        {
            foreach (Option option in conversation.optionsList)
            {
                // Adding the option only if it doesn't end the dialogue
                if(!(option.endsDialogue))
                {
                    playerTexts[textIterator] = option.playerText;
                }
            }
        }

        //Debug.Log(conversationList.Count);

        // Creating conversation iterator which will be used in functions as a parameter
        int conversationIterator = 0;
        // For each conversation displaying text fields and options associated to that conversation
        foreach(Conversation conversation in conversationList)
        {
            if(conversationIterator == 0)
            {
                // If it's the first conversation the user is able to add options to this conversation
                allowToAddOption = true;

                // What the speaker (NPC) is supposed to say at the start of this conversation
                speakerText = EditorGUILayout.TextField("Speaker (NPC) start text: ", conversation.speakerText);

                // Adding the speakerText to the very first conversation
                conversationList[0].conversationID = "start";
                conversationList[0].speakerText = this.speakerText;
                conversationList[0].speakerName = this.speakerName;
                // Clearing speakerText after it has been added
                this.speakerText = "";
            }
            else
            {
                EditorGUILayout.LabelField("What does the player say to start this conversation?");

                // TU BEDA PROBLEMY!@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
                // The user needs to select which player's text started this conversation
                popUpIndex = EditorGUILayout.Popup(conversationList[conversationIterator].popupIndex, playerTexts);

                // Assigning the value to the conversation
                conversationList[conversationIterator].popupIndex = this.popUpIndex;

                // If nothing was selected from the list it won't allow the user to add any options to this conversation
                if (popUpIndex == 0)
                    allowToAddOption = false;
                else
                    allowToAddOption = true;

                speakerText = EditorGUILayout.TextField("Speaker (NPC) text: ", speakerText);
            }

            int optionIterator = 0;
            // Displaying the options the player will be able to choose from
            foreach(Option option in conversationList[conversationIterator].optionsList)
            {
                // Used to make the code a little bit more clear
                string nextOption = "";

                // If it's the first conversation options should be laid out like this: option1, option2, option3...
                if(conversationIterator == 0)
                {
                    nextOption = "option" + (optionIterator + 1);
                    // Assigning the ID
                    option.nextConversationID = nextOption;
                }
                else
                {
                    // If it's another conversation (2nd, 3rd...) options are layed out like this: 
                    // option1.1, option1.2 or option2.1, option2.2 or option2.1.1, option2.1.2 and so on
                    // depending on which player's text leads here

                    // Getting the ID of the current conversation
                    string currentID = conversationList[conversationIterator].conversationID;
                    
                    // Adding a dot and optionNumber to the ID
                    nextOption = currentID + "." + (optionIterator + 1);

                    // Assigning the ID
                    option.nextConversationID = nextOption;
                }

                /*
                GUILayout.BeginHorizontal();
                playerText = EditorGUILayout.TextField("Player text: ", playerDict[optionID]);
                endBtn = EditorGUILayout.Toggle("Ends dialogue?", playerTextEndsDialogue[optionID]);
                GUILayout.EndHorizontal();

                // Assigning value to the dictionaries
                playerDict[optionID] = playerText;
                playerTextEndsDialogue[optionID] = endBtn;

                // Clearing playerText after adding it to the dictionary
                playerText = "";
                */
                optionIterator++;
            }


            conversationIterator++;
        }

        /*
        // Creating a dictionary iterator which will be used in functions as a key of conversationDict dictionary
        int dictIterator = 0;
        // For each conversation displaying text fields and options accociated to that conversation
        foreach (int key in conversationDict.Keys)
        {
            if (dictIterator == 0)
            {
                
            }
            else
            {

            }

            // Displaying options the player will be able to choose from
            for (int i = 0; i < conversationDict[key]; i++)
            {

                if (dictIterator == 0)
                {

                }
                else
                {

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
            if (GUILayout.Button("Add option to conversation "))
            {
                // The user can add option only if he selected which player's text starts this conversation
                if (allowToAddOption)
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
         }*/

        // If add conversation button was pressed
        if (GUILayout.Button("Add conversation " + conversationIterator))
        {
            AddConversation();
        }

        // If remove conversation button was pressed
        if (GUILayout.Button("Remove conversation"))
        {
            //if (EditorUtility.DisplayDialog("Remove conversation?", "Are you sure you want to remove last conversation?", "Remove", "Cancel"))
                //RemoveConversation();
        }

        // Created to add space, reducing the error of pressing accidentaly to clear all conversations
        EditorGUILayout.LabelField("");
        // If clear conversations button was pressed
        if (GUILayout.Button("Clear conversations"))
        {
            //if (EditorUtility.DisplayDialog("Clear all conversations?", "Are you sure you want to clear all conversations?", "Clear all", "Cancel"))
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