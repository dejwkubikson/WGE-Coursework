using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataScript : MonoBehaviour
{
	public string fileName = "";

    // Start is called before the first frame update
    void Start()
    {
		DontDestroyOnLoad (this);
	
		// getting the current scene
		Scene currentScene = SceneManager.GetActiveScene ();

		// retrieving the name of current scene
		string sceneName = currentScene.name;

		// passing the file name to voxelChunk script if it's the right scene
		//if (sceneName == "Scene1")
			//voxelchunk filetoLoad = fileName;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
