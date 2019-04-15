using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Xml;

// This script should be attached to an empty game object. Looks for approporiate conversation that should be displayed and was choosen by the player.
public class DialogueScript : MonoBehaviour
{
    public GameObject dialogueWindow; // dialogue window that holds the whole GUI of conversation
    public GameObject optionField; // option field is the field that the player can press to choose a dialogue option
    private CameraScript cameraScript;
    public string dialogueFileName = "DialogueData.xml";
    public bool dialogueStarted = false; // true if the dialogue started
    public bool dialogueEnded = false; // true if the dialogue ended
    public bool allOptionsShown = false; // true when all the options for the player were shown
    public Dictionary<string, string> options; // dictionary holding the key (option that will open the next appropriate conversation) and value (text that is associated with this option)

    private float displayTextDelay = 1.0f; // the time delay before displaying another option for the player
    private float displayCharDelay = 0.1f; // the time delay before displaying another character of the text the speaker is saying

    // Starts the dialogue, opens dialogue window, zooms the camera and load xml file with dialogue
    public void StartDialogue()
    {
        dialogueStarted = true;
        dialogueWindow.SetActive(true);

        // Getting the camera zoom and focus on NPC
        cameraScript.zoomDesired = 1.5f;
        cameraScript.zoomCamera = true;

        LoadDialogueFromXMLFile(dialogueFileName, "start");
    }

    // Ends the dialogue, sets the camera to be 'free' again and closes dialogue window
    public void EndDialogue()
    {
        dialogueStarted = false;
        dialogueEnded = true;

        // Setting the camera to be 'free'
        cameraScript.cameraComingFromZoom = true;
        cameraScript.zoomCamera = false;
        // This is not using SwitchFocus() to be sure 100% that the camera will follow the player after the dialogue ends
        cameraScript.followNPC = false;
        cameraScript.followPlayer = true;

        dialogueWindow.SetActive(false);
    }

    // Loads dialogue from file with approporiate conversation, displays the text, saves the options into the dictionary
    public void LoadDialogueFromXMLFile(string fileName, string conversationID)
    {
        // Checking if the file has XML ending
        if (!(fileName.Contains(".xml")))
            fileName += ".xml";

        // Checking if the file exists
        if (!(System.IO.File.Exists(fileName)))
        {
            Debug.Log("File " + fileName + " wasn't found!");
            return;
        }

        // Debug.Log("Loaded dialogue, looking for " + conversationID);

        // Create an XML reader with the file supllied
        XmlReader xmlReader = XmlReader.Create(fileName);

        // No need to check if the file exists because it was done before
        while(xmlReader.Read())
        {
            // If there is a <conversation> node
            if(xmlReader.IsStartElement("conversation"))
            {
                // Getting the id of the conversation in XML file
                string id = xmlReader["id"];
                // Debug.Log("Conversation ID is " + id);
                // Searching for the right conversation id
                if(id == conversationID)
                {
                    // Erasing any previous data from the options dictionary
                    options.Clear();

                    // Getting another node
                    xmlReader.Read();
                    // <text>
                    // Getting text and it's speaker
                    xmlReader.ReadToNextSibling("text");
                    string speaker = xmlReader["speaker"];
                    xmlReader.Read();
                    // Assigning the text to a variable
                    string speakerText = xmlReader.Value;
                    // </text>
                    // Debug.Log("Speaker name: " + speaker + ", text: " + speakerText);
                    // Moving to another node
                    xmlReader.Read();
                    // <options>
                    xmlReader.ReadToNextSibling("options");
                    xmlReader.Read();

                    // Reading through the options
                    // <option>
                    // While there are <option> nodes in the options
                    while (xmlReader.ReadToNextSibling("option"))
                    {
                        string nextOption = xmlReader["next"];
                        string textOption = xmlReader.ReadElementContentAsString();
                        // Adding the id of the option and its text to the dictionary
                        options.Add(nextOption, textOption);
                    }
                    // </option>
                    // </options>

                    // Displaying the dialogue
                    StartCoroutine(DisplayDialogue(speaker, speakerText, options));

                }// end of if(id == conversationID)
            }// end of if(IsStartElement("conversation")
        }// end of while Read()
    }// end of LoadDialogueFromXMLFile()

    // Coroutine that displays the dialogue in the GUI, creates option field
    IEnumerator DisplayDialogue(string speakerName, string speakerText, Dictionary<string, string> options)
    {
        allOptionsShown = false;
        cameraScript.SwitchCameraFocus();
        // Getting the speaker name and his text Text fields, assigning the values to them
        Text speakerNameTxt = GameObject.Find("Speaker").gameObject.GetComponent<Text>();
        speakerNameTxt.text = speakerName;
        Text speakerTextTxt = GameObject.Find("SpeakerText").gameObject.GetComponent<Text>();
        // Erasing any previous text from the text field
        speakerTextTxt.text = "";
        
        // Displaying speakers text with a delay for each character
        for(int i = 0; i < speakerText.Length; i++)
        {
            speakerTextTxt.text += speakerText[i];
            yield return new WaitForSeconds(displayCharDelay);
        }

        float yOffset = 30.0f;
        // Hold the index of the current position in the dictionary
        int index = 0;
        // Displaying player's options with delay
        foreach(string value in options.Values)
        {
            // Creating a duplicate of the option field
            GameObject newOptionField = Instantiate(optionField) as GameObject;
            // Setting the new option name as key, this will be used as a reference of what the player chooses
            foreach(string key in options.Keys)
            {
                if(options[key].Equals(value))
                {
                    newOptionField.name = key;
                }
            }
            // Making the dialogue window parent
            newOptionField.transform.SetParent(dialogueWindow.transform);
            // Option field is disabled therefore the new item needs to be enabled
            newOptionField.SetActive(true);
            // Attaching script to the new object
            newOptionField.AddComponent<OptionScript>();
            // Assigning position to the new option
            newOptionField.transform.position = new Vector3(Screen.width / 2, (133 - (yOffset * index)), 0.0f);
            // Getting the text component
            Text newOptionFieldTxt = newOptionField.gameObject.GetComponent<Text>();
            // Setting the value of the text field with prefix of option number and '.'
            newOptionFieldTxt.text = (index + 1) + ". " + value;
            index++;

            // No delay after the last option is shown
            if (index < options.Count)
                yield return new WaitForSeconds(displayTextDelay);
        }
        allOptionsShown = true;
    }

    // This function is only used to start the coroutine. This will allow the option fields to be destroyed before the whole text shows up.
    public void ChooseDialogueOptionStarter(string chosenOptionName)
    {
        StartCoroutine(ChooseDialogueOption(chosenOptionName));
    }

    // Coroutine that checks which option the player chose. Displays the chosen text in the GUI and starts function that searches for new conversation
    public IEnumerator ChooseDialogueOption(string chosenOptionName)
    {
        // Display new name of speaker
        Text speakerNameTxt = GameObject.Find("Speaker").gameObject.GetComponent<Text>();
        speakerNameTxt.text = GameObject.FindGameObjectWithTag("Player").name;
        // Make camera follow new speaker
        cameraScript.SwitchCameraFocus();
        // Show chosen option as speaker text
        Text speakerTextTxt = GameObject.Find("SpeakerText").gameObject.GetComponent<Text>();
        // Erasing any previous data in the text field
        speakerTextTxt.text = "";
        string speakerText = options[chosenOptionName];

        // Deleting all created option fields
        foreach(string key in options.Keys)
        {
            Destroy(GameObject.Find(key).gameObject);
        }

        // Displaying speakers text with a delay for each character
        for (int i = 0; i < speakerText.Length; i++)
        {
            speakerTextTxt.text += speakerText[i];
            yield return new WaitForSeconds(displayCharDelay);
        }

        // Another delay before switching to another conversation
        yield return new WaitForSeconds(2.0f);

        if (chosenOptionName == "end")
            EndDialogue();
        else
            LoadDialogueFromXMLFile(dialogueFileName, chosenOptionName);
    }

    // Start is called before the first frame update
    void Start()
    {
        // Creating a new dictionary that will hold option's id and its text
        options = new Dictionary<string, string>();
        cameraScript = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraScript>();
    }
}
