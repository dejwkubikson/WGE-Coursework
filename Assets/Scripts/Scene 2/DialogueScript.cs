using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class DialogueScript : MonoBehaviour
{
    public string dialogueFileName = "DialogueData.xml";
    public bool dialogueStarted = false;
    public bool dialogueEnded = false;


    private string speaker = "";

    public void StartDialogue()
    {
        dialogueStarted = true;

        LoadDialogueFromXMLFile(dialogueFileName);
    }

    public void LoadDialogueFromXMLFile(string fileName)
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

        // Create an XML reader with the file supllied
        XmlReader xmlReader = XmlReader.Create(fileName);

        while(xmlReader.Read())
        {
            // Getting the name of the speaker
            if(xmlReader.IsStartElement("speaker"))
            {
                speaker = xmlReader.Value;
                Debug.Log("Speaker name: " + speaker);
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

        // Create ab XML reader with the file supplied
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
