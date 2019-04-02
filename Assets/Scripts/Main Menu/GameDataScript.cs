using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// This script is used to pass the XML file name to VoxelChunk
public class GameDataScript : MonoBehaviour
{
	public string fileName = "";

    // Start is called before the first frame update
    void Start()
    {
		DontDestroyOnLoad (this);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
