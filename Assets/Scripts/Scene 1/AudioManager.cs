using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioClip destroyBlockSound;
    public AudioClip placeBlockSound;

    // play the destroy block sound
    void PlayBlockSound(int blockType)
    {
        if(blockType == 0)
            GetComponent<AudioSource>().PlayOneShot(destroyBlockSound);
        else
            GetComponent<AudioSource>().PlayOneShot(placeBlockSound);
    }

   /* void PlayPlaceBlockSound()
    {
        GetComponent<AudioSource>().PlayOneShot(placeBlockSound);
    }*/

    // when game object is enabled
    private void OnEnable()
    {
        VoxelChunk.OnEventBlockChanged += PlayBlockSound;
        //VoxelChunk.OnEventBlockDestroyed += PlayDestroyBlockSound;
        //VoxelChunk.OnEventBlockPlaced += PlayPlaceBlockSound;
    }

    private void OnDisable()
    {
        VoxelChunk.OnEventBlockChanged -= PlayBlockSound;
        //VoxelChunk.OnEventBlockDestroyed -= PlayDestroyBlockSound;
        //VoxelChunk.OnEventBlockPlaced -= PlayPlaceBlockSound;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
