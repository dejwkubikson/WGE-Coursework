using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// This script runs the main menu, loads scenes, get input from the input field. Should be attached to an empty game object.
public class MainMenuScript : MonoBehaviour
{
	public InputField inputXMLfile;
	public Text errorText;

	private string fileName = "";
	private bool fileFound = false;

	public void LoadScene1()
	{
		GameObject gameData = GameObject.Find ("GameDataObject");
		
        // Double checking, object should exists from the Start()
		if (gameData != null) 
		{
			GameDataScript gameDataScript = gameData.GetComponent<GameDataScript> ();

			fileName = inputXMLfile.text;

			// If the input field isn't empty
			if (fileName != "") 
			{
				// If the file exists and the user wrote it with .xml
				if (System.IO.File.Exists (fileName))
					fileFound = true;
				else // If the file exists and the user wrote the name of the file without .xml
					if (System.IO.File.Exists (fileName + ".xml"))
					{
						fileFound = true;
						fileName += ".xml"; // Yhe user wrote file name without .xml so it needs to be added
					}else
						fileFound = false;
			} 
			else
				fileFound = false;
			
			// Assigning the file name, this then will be passed onto VoxelChunk script at the start of the scene
			if (fileFound) 
			{
				gameDataScript.fileName = fileName;
				errorText.color = Color.green;
				errorText.text = "Loading " + fileName + " file";
			}
			else
			{	
				gameDataScript.fileName = "AssessmentChunk1.xml"; // Loading default chunk because the file wasn't found

				// Displaying the 'error' message. If the user didn't enter anything in the input field it means that he just wants to play with the default chunk, therefore the message is more of an information of what's happening
				if (fileName != "") 
				{
					errorText.color = Color.red;
					errorText.text = "Couldn't find the file. Loading default chunk.";
				}
                else
                {
                    errorText.color = Color.green;
                    errorText.text = "Loading default chunk.";
                }
			}
		}

		StartCoroutine (DelayLoadScene1 ());
	}

	// Used so that the user sees the error message
	IEnumerator DelayLoadScene1()
	{
		yield return new WaitForSeconds (3.0f);
        SceneManager.LoadScene("Scene1");
	}

	public void LoadScene2()
	{
        SceneManager.LoadScene("Scene 2");
	}

	public void QuitGame()
	{
		Application.Quit ();
	}

    // Start is called before the first frame update
    void Start()
    {
		// Creating a GameDataObject object with GameData script  if it doesn't exist
		GameObject gameData = GameObject.Find ("GameDataObject");
		if (gameData == null) {
			gameData = new GameObject ("GameDataObject");
			gameData.AddComponent<GameDataScript> ();
		}
    }
}
