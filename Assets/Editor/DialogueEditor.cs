using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Xml;

// Conversation class
public class Conversation
{
    public string conversationID;
    public string speakerText;
    public string speakerName;
    public int popupIndex; // just for display purposes in the select list
    public List<Option> optionsList = new List<Option>();
}

// Option class
public class Option
{
    public string nextConversationID;
    public string playerText;
    public bool endsDialogue;
}

// This scripts lets the developer create new dialogues or edit current ones. Moreover, dialogues can be loaded into the scene and tested.
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

    // Adds a new conversation to the conversationList
    private void AddConversation()
    {
        Conversation conversation = new Conversation();
        conversation.conversationID = "";
        conversation.speakerText = "";
        conversation.speakerName = this.speakerName;
        conversation.popupIndex = 0;
        conversationList.Add(conversation);
    }

    // Removes a certain conversation
    private void RemoveConversation(int whichConversation)
    {
        conversationList.RemoveAt(whichConversation);
    }

    // Clears all conversations
    private void ClearConversations()
    {
        conversationList.Clear();
    }

    // Adds an option to certain conversation and creates option's ID
    private void AddOption(int whichConversation)
    {
        Option option = new Option();
        option.playerText = "";

        // Creating appropriate ID, if it's the first conversation options should look like this: option1, option2, option3...
        if (whichConversation == 0)
            option.nextConversationID = "option" + (conversationList[whichConversation].optionsList.Count + 1);
        else
            // If it's another conversation (2nd, 3rd...) options should look like this: 
            // option1.1, option1.2 or option2.1, option2.2 or option2.1.1, option2.1.2 and so on
            option.nextConversationID = conversationList[whichConversation].conversationID + "." + (conversationList[whichConversation].optionsList.Count + 1);

        option.endsDialogue = false;

        conversationList[whichConversation].optionsList.Add(option);
    }

    // Removes certain option in certain conversation
    private void RemoveOption(int whichConversation, int whichOption)
    {
        // Removing last option from chosen conversation
        conversationList[whichConversation].optionsList.RemoveAt(whichOption);
    }
    
    // Loads dialogue to the inspector
    private void LoadDialogueToInspector(string fileName)
    {
        // Checking if the file has XML ending
        if (!(fileName.Contains(".xml")))
            fileName += ".xml";

        // Checking if the file exists
        if (!(System.IO.File.Exists(fileName)))
        {
            EditorUtility.DisplayDialog("File not found!", "File " + fileName + " could not be found. Please check if you have entered the correct file name.", "Ok");
            return;
        }

        // Create an XML reader with the file supllied
        XmlReader xmlReader = XmlReader.Create(fileName);

        // Created for the use of popupIndex - it will properly show the selected option in select lists
        int xmlIterator = 0;
        // No need to check if the file exists because it was done before
        while (xmlReader.Read())
        {
            // If there is a <conversation> node
            if(xmlReader.IsStartElement("conversation"))
            {
                // Getting the id of the conversation in XML file
                string id = xmlReader["id"];
                // Moving to another node
                xmlReader.Read();
                // <text>
                xmlReader.ReadToNextSibling("text");
                string speaker = xmlReader["speaker"];
                xmlReader.Read();
                // Assigning the text to a variable
                string npcText = xmlReader.Value;
                // </text>

                // Creating new Conversation
                Conversation conversation = new Conversation();
                conversation.conversationID = id;
                conversation.speakerName = speaker;
                conversation.speakerText = npcText;
                conversation.popupIndex = xmlIterator;
                // Adding the speaker name to the inspector
                this.speakerName = speaker;

                // Moving to another node
                xmlReader.Read();
                // <options>
                xmlReader.ReadToNextSibling("options");
                xmlReader.Read();

                // Reading through the options
                // <option>
                int optionIterator = 1;
                while (xmlReader.ReadToNextSibling("option"))
                {
                    string nextOption = xmlReader["next"];
                    string textOption = xmlReader.ReadElementContentAsString();
                    // Creating new Option element
                    Option option = new Option();
                    option.nextConversationID = nextOption;
                    option.playerText = textOption;
                    // Checking if the option's 'ends the dialogue' checkbox should be checked
                    if (nextOption == "end")
                    {
                        option.endsDialogue = true;
                        // Creating a dummy option ID so that it doesn't show up as 'end' option
                        option.nextConversationID = conversation.conversationID + "." + optionIterator;
                    }
                    // Adding to the conversations options list
                    conversation.optionsList.Add(option);
                    optionIterator++;
                }
                // </option>
                // </options>

                // Adding the conversation element to conversationList
                conversationList.Add(conversation);
                xmlIterator++;
            }// end of IsStartElement("conversation")
        }// end of while Read()
    }// end of LoadDialogueToInspector()

    // Loads dialogue to the scene 
    private void LoadDialogueToScene(string fileName)
    {
        DialogueScript dialogueScript;
        dialogueScript = GameObject.Find("DialogueObject").GetComponent<DialogueScript>();
        dialogueScript.dialogueFileName = fileName;
    }

    // Creates a new dialogue
    private void CreateDialogue(string fileName)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        // Creating file, if the user didn't type the extension adding it for him
        if (!(fileName.Contains(".xml")))
            fileName += ".xml";

        XmlWriter xmlWriter = XmlWriter.Create(fileName, writerSettings);

        // Starting the document
        xmlWriter.WriteStartDocument();

        // Creating root
        xmlWriter.WriteStartElement("Dialogue");
        foreach(Conversation conversation in conversationList)
        {
            // Creating conversation element
            // <conversation>
            xmlWriter.WriteStartElement("conversation");
            // Writing the ID of this conversation
            xmlWriter.WriteAttributeString("id", conversation.conversationID.ToString());
            
            // <text>
            // Writing the speaker name as attribute and his text in <text> element
            xmlWriter.WriteStartElement("text");
            xmlWriter.WriteAttributeString("speaker", conversation.speakerName.ToString());
            xmlWriter.WriteString(conversation.speakerText.ToString());
            // Ending the text element
            xmlWriter.WriteEndElement();
            // </text>

            // <options>
            xmlWriter.WriteStartElement("options");
            // Creating the player's options
            foreach(Option option in conversation.optionsList)
            {
                // <option>
                xmlWriter.WriteStartElement("option");

                // Changing the nextConversationID here. It's easier to manipulate the IDs here
                if (option.endsDialogue)
                    option.nextConversationID = "end";

                xmlWriter.WriteAttributeString("next", option.nextConversationID.ToString());
                xmlWriter.WriteString(option.playerText.ToString());
                xmlWriter.WriteEndElement();
                // </option>
            } // end of foreach option
            xmlWriter.WriteEndElement();
            // </options>
            xmlWriter.WriteEndElement();
            // </conversation>
        } // end of foreach conversation

        // Ending the root
        xmlWriter.WriteEndElement();
        // Ending the document
        xmlWriter.WriteEndDocument();
        // Closing the document
        xmlWriter.Close();
    } // end of CreateDialogue()

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
        foreach (Conversation conversation in conversationList)
        {
            foreach (Option option in conversation.optionsList)
            {
                numberOfPlayerOptions++;
            }
        }

        // Storing player texts in a new array, first element is constant as it will be a default element in select lists
        string[] playerTexts = new string[numberOfPlayerOptions + 1];
        playerTexts[0] = "Please select one of the options";

        // Starting the textIterator from 1 as it will be the index of the playerTexts array
        int textIterator = 1;

        // Adding the options the player can choose to playerTexts array
        foreach (Conversation conversation in conversationList)
        {
            foreach (Option option in conversation.optionsList)
            {
                // Adding 'ENDS DIALOGUE - ' text so that the user knows to not to choose it as a conversation starter
                if (option.endsDialogue)
                {
                    playerTexts[textIterator] = option.nextConversationID + " ENDS DIALOGUE - " + option.playerText;
                }
                else
                {
                    if (option.playerText == "")
                        playerTexts[textIterator] = "EMPTY OPTION WITH ID: " + option.nextConversationID;
                    else
                        playerTexts[textIterator] = option.nextConversationID + ": " + option.playerText;
                }

                textIterator++;
            }
        }

        // Creating conversation iterator which will be used in functions as a parameter
        int conversationIterator = 0;
        // For each conversation displaying text fields and options associated to that conversation
        foreach (Conversation conversation in conversationList)
        {
            if (conversationIterator == 0)
            {
                // If it's the first conversation the user is able to add options to this conversation
                allowToAddOption = true;

                // What the speaker (NPC) is supposed to say at the start of this conversation
                speakerText = EditorGUILayout.TextField("Speaker (NPC) start text: ", conversation.speakerText);

                // Adding the speakerText to the very first conversation
                conversationList[0].conversationID = "start";
                conversationList[0].speakerText = this.speakerText;
                // Clearing speakerText after it has been added / shown
                this.speakerText = "";
            }
            else
            {
                EditorGUILayout.LabelField("What does the player say to start this conversation?");

                // The user needs to select which player's text started this conversation
                popUpIndex = EditorGUILayout.Popup(conversation.popupIndex, playerTexts);

                // Checking if the user didn't choose a text that ends the dialogue
                if (playerTexts[popUpIndex].Contains("ENDS DIALOGUE - "))
                {
                    EditorUtility.DisplayDialog("Option ends dialogue!", "Options that end dialogue cannot be used as conversation starters.", "Ok");
                    popUpIndex = 0;
                }

                // If nothing was selected from the list it won't allow the user to add any options to this conversation
                if (popUpIndex == 0)
                    allowToAddOption = false;
                else
                    allowToAddOption = true;

                speakerText = EditorGUILayout.TextField("Speaker (NPC) text: ", conversation.speakerText);
                // Assigning the value to the conversation
                conversation.popupIndex = this.popUpIndex;
                // Assigning the conversationID based on what option started this conversation - ID based on string split
                conversation.conversationID = playerTexts[popUpIndex].Split(':')[0];
                conversation.speakerText = this.speakerText;

                // Clearing speakerText after it has been added / shown
                this.speakerText = "";
            }

            int optionIterator = 0;
            // Displaying the options the player will be able to choose from
            foreach (Option option in conversation.optionsList)
            {
                GUILayout.BeginHorizontal();
                playerText = EditorGUILayout.TextField("Player text: ", option.playerText, GUILayout.Width(Screen.width / 3 * 2));
                if (GUILayout.Button("Remove", GUILayout.Width(Screen.width / 12 * 1)))
                {
                    if (EditorUtility.DisplayDialog("Remove option?", "Are you sure you want to remove this option?", "Remove", "Cancel"))
                    {
                        RemoveOption(conversationIterator, optionIterator);
                        // Stopping the foreach loop as the list could go out of sync. Therefore the list won't change while looping which could end up in an infinite loop.
                        break;
                    }
                }
                endBtn = EditorGUILayout.Toggle("Ends dialogue?", option.endsDialogue, GUILayout.Width(Screen.width / 3 * 1));
                GUILayout.EndHorizontal();

                // Assigning value to the dictionaries
                option.playerText = this.playerText;
                option.endsDialogue = endBtn;

                // Clearing after values have beed added / shown
                this.playerText = "";
                endBtn = false;

                optionIterator++;
            }

            GUILayout.BeginHorizontal();
            // If the user presses a button to add option
            if (GUILayout.Button("Add option to conversation", GUILayout.Width(Screen.width / 4 * 3)))
            {
                // The user can add option only if he selected which player's text starts this conversation
                if (allowToAddOption)
                    AddOption(conversationIterator);
                else
                    EditorUtility.DisplayDialog("Select text first!", "Before adding any options please select text that brings the player to this conversation.", "Ok");

                // Stopping the foreach loop as the list could go out of sync. Therefore the list won't change while looping which could end up in an infinite loop.
                break;
            }

            // If there is more than 1 conversation adding remove conversation option
            if (conversationIterator > 1)
            {
                // If remove conversation button was pressed
                if (GUILayout.Button("Remove conversation", GUILayout.Width(Screen.width / 4 * 1)))
                {
                    if (EditorUtility.DisplayDialog("Remove conversation?", "Are you sure you want to remove this conversation?", "Remove", "Cancel"))
                    {
                        RemoveConversation(conversationIterator);
                        // Stopping the foreach loop as the list could go out of sync. Therefore the list won't change while looping which could end up in an infinite loop.
                        break;
                    }
                }
            }
            GUILayout.EndHorizontal();

            // Created to add space between each conversation, makes it clearer for the user which conversation he is at
            EditorGUILayout.LabelField("");

            conversationIterator++;
        }

        // Created to add space between each conversation, makes it clearer for the user which conversation he is at
        EditorGUILayout.LabelField("");

        // If add conversation button was pressed
        if (GUILayout.Button("Add conversation"))
        {
            AddConversation();
        }

        // Created to add space, reducing the error of pressing accidentaly to clear all conversations
        EditorGUILayout.LabelField("");

        // If clear conversations button was pressed
        if (GUILayout.Button("Clear conversations"))
        {
            //if (EditorUtility.DisplayDialog("Clear all conversations?", "Are you sure you want to clear all conversations?", "Clear all", "Cancel"))
            ClearConversations();
        }

        // Showing the user where the dialogue editing ends
        EditorGUILayout.LabelField("End of the dialogue");

        // When create dialogue button is pressed
        if (GUILayout.Button("Create dialogue"))
        {
            CreateDialogue(dialogueFileNameToSave);
        }

        // Created to add space to make it more clear that this section doesn't belong to the dialogue editor
        EditorGUILayout.LabelField("");
        EditorGUILayout.LabelField("Load dialogue from file");
        dialogueFileNameToLoad = EditorGUILayout.TextField("Name of file to load: ", dialogueFileNameToLoad);

        // If the user wants to load the dialogue to the inspector
        if (GUILayout.Button("Load dialogue to inspector"))
        {
            LoadDialogueToInspector(dialogueFileNameToLoad);
        }

        // If the user wants to load the dialogue to the scene
        if (GUILayout.Button("Load dialogue to scene"))
        {
            LoadDialogueToScene(dialogueFileNameToLoad);
        }

        // Created to add space at the bottom
        EditorGUILayout.LabelField("");
        EditorGUILayout.EndScrollView();
    }

    void OnInspectorUpdate()
    {
        Repaint();
    }
}