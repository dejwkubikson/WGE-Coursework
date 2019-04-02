using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class DialogueScript : MonoBehaviour
{
    public string dialogueFileName = "DialogueData.xml";
    public bool dialogueStarted = false;
    public bool dialogueEnded = false;
    public Dictionary<string, string> options;

    public void StartDialogue()
    {
        dialogueStarted = true;

        LoadDialogueFromXMLFile(dialogueFileName, "start");
    }

    public void EndDialogue()
    {
        dialogueStarted = false;
        dialogueEnded = true;
    }

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

        Debug.Log("Loaded dialogue, looking for " + conversationID);

        // Create an XML reader with the file supllied
        XmlReader xmlReader = XmlReader.Create(fileName);

        while(xmlReader.Read())
        {
            // Getting the id of the conversation
            if(xmlReader.IsStartElement("conversation"))
            {
                string id = xmlReader["id"];
                //Debug.Log("Conversation ID is " + id);
                // Searching for the right conversation id
                if(id == conversationID)
                {
                    // Getting another node
                    xmlReader.Read();
                    // <text>
                    // Getting text and it's speaker
                    xmlReader.ReadToNextSibling("text");
                    string speaker = xmlReader["speaker"];
                    xmlReader.Read();
                    // Assigning the text to a variable
                    string text = xmlReader.Value;
                    // </text>
                    Debug.Log("Speaker name: " + speaker + ", text: " + text);
                    // Moving to another node
                    xmlReader.Read();
                    // <options>
                    xmlReader.ReadToNextSibling("options");
                    xmlReader.Read();

                    // Reading through the options
                    // <option>
                    int numberOfOptions = 0;
                    
                    // While there are <option> nodes in the options
                    while (xmlReader.ReadToNextSibling("option"))
                    {
                        numberOfOptions++;

                        string nextOption = xmlReader["next"];
                        string textOption = xmlReader.ReadElementContentAsString();

                        Debug.Log("Nxt option: " + nextOption + ", text: " + textOption);
                        // Adding the id of the option and text to the dictionary
                        //options.Add()
                    }

                    // </option>
                    Debug.Log("number of options" + numberOfOptions);

                    // </options>

                    // Getting another line 


                    

                }

                //speaker = xmlReader.Value;
                //Debug.Log("Speaker name: " + speaker);
            }

        }

    }
    /*
    <speaker>NPC</speaker>
	
	<conversation id = "start" >

        < text id="start">Hey, how's it going?</text>
		<options>
			<option next = "option1" > I'm doing okay.</option>
			<option next = "option2" > I'm doing terribly.</option>
			<option next = "option3" > I'm doing Great!</option>
		</options>
	</conversation>
		
	<conversation id = "option1" >

        < text > That's good to hear.</text>
		<options>
			<option next = "end" > Bye.</ option >

        </ options >

    </ conversation >
    */
    // Read a voxel chunk from XML file
    public static int[,,] LoadChunkFromXMLFile(int size, string fileName)
    {
        int[,,] voxelArray = new int[size, size, size];

        // Create an XML reader with the file supplied
        XmlReader xmlReader = XmlReader.Create(fileName);

        // Iterate through and read every line in the XML file
        while (xmlReader.Read() && System.IO.File.Exists(fileName))
        {
            if (xmlReader.IsStartElement("Voxel"))
            {
                // Retrieve x, y and z attributes and store as int
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);

                xmlReader.Read(); // Goes to another node!

                int value = int.Parse(xmlReader.Value);

                voxelArray[x, y, z] = value;
            }
        }
        return voxelArray;
    }

    // Start is called before the first frame update
    void Start()
    {
        options = new Dictionary<string, string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
