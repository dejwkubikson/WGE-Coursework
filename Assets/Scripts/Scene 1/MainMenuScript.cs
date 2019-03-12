using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuScript : MonoBehaviour
{
	public InputField inputXMLfile;
	public Text errorText;

	private string fileName = "";
	private bool fileFound = false;

	public void LoadScene1()
	{
		GameObject gameData = GameObject.Find ("GameDataObject");
		// double checking, object should exists from the Start()
		if (gameData != null) 
		{
			GameDataScript gameDataScript = gameData.GetComponent<GameDataScript> ();

			fileName = inputXMLfile.text;

			// if the input field isn't empty
			if (fileName != "") 
			{
				// if the file exists and the user wrote it with .xml
				if (System.IO.File.Exists (fileName))
					fileFound = true;
				else // if the file exists and the user wrote the name of the file without .xml
					if (System.IO.File.Exists (fileName + ".xml"))
					{
						fileFound = true;
						fileName += ".xml"; // the user wrote file name without .xml so it needs to be added
					}else
						fileFound = false;
			} 
			else
				fileFound = false;
			
			// assigning the file name, this then will be passed onto VoxelChunk script at the start of the scene
			if (fileFound) 
			{
				gameDataScript.fileName = fileName;
				errorText.color = Color.green;
				errorText.text = "Loading " + fileName + " file";
			}
			else
			{	
				gameDataScript.fileName = "AssessmentChunk1.xml"; // loading default chunk because the file wasn't found

				// I don't want to display error message when the user just wants to play with default chunk and doesn't enter anything in the input field
				if (fileName != "") 
				{
					errorText.color = Color.red;
					errorText.text = "Couldn't find the file. Loading default chunk.";
				}
			}
		}

		// used so that the user sees the error message
		StartCoroutine (DelayLoadScene1 ());
	}

	// used so that the user sees the error message
	IEnumerator DelayLoadScene1()
	{
		yield return new WaitForSeconds (3.0f);
		Application.LoadLevel ("Scene1");
	}

	public void LoadScene2()
	{
		Application.LoadLevel ("Scene 2");
	}

	public void QuitGame()
	{
		Application.Quit ();
	}

    // Start is called before the first frame update
    void Start()
    {
		// creating a GameDataObject object with GameData script  if it doesn't exist
		GameObject gameData = GameObject.Find ("GameDataObject");
		if (gameData == null) {
			gameData = new GameObject ("GameDataObject");
			gameData.AddComponent<GameDataScript> ();
		}
    }

    // Update is called once per frame
    void Update()
    {
		
    }
}
