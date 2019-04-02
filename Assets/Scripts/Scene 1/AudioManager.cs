using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    public AudioClip destroyBlockSound;
    public AudioClip placeBlockSound;

    // Plays the destroy / place block sound
    void PlayBlockSound(int blockType)
    {
        if(blockType == 0)
            GetComponent<AudioSource>().PlayOneShot(destroyBlockSound);
        else
            GetComponent<AudioSource>().PlayOneShot(placeBlockSound);
    }

    // When game object is enabled
    private void OnEnable()
    {
        VoxelChunk.OnEventBlockChanged += PlayBlockSound;
    }

    private void OnDisable()
    {
        VoxelChunk.OnEventBlockChanged -= PlayBlockSound;
    }
}
