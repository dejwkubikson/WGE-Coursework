using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script is attached to a collectable. It plays sounds, destroys when is collected and can be drawn towards the player.
public class CollectableScript : MonoBehaviour
{
    GameObject player; // Used to get current position
    GameObject camera; // The collectable moves to the camera so that it doesn't just move on the ground but goes to the player's face so that he clearly can see picking it up
    public int blockType; 
    public bool moveTowards = false; // if the player is close enough to the collectable it should move towards him even if he starts to move away from it

    private AudioSource audioSource;
    private bool coroutineNotPlayed = true;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = GameObject.FindGameObjectWithTag("MainCamera");
        audioSource = player.GetComponent<AudioSource>();
    }

    IEnumerator PickUpAndDestroy()
    {
        coroutineNotPlayed = false;
        InventoryScript inventory = player.GetComponent<InventoryScript>();
        inventory.AddItemToInventory(blockType);
        AudioClip pickUpSound = Resources.Load<AudioClip>("Sounds/pick_up_sound");
        audioSource.PlayOneShot(pickUpSound);
        yield return new WaitForSeconds(0.2f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        // Making the collectable rotate
        transform.Rotate(Vector3.up * 2);

        // If the distance between the player and collectable is small the block will move towards the player
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if(dist <= 2.5f)
        {
            moveTowards = true;

            // If the object is very close then it is picked up
            if (dist < 1.0f && coroutineNotPlayed)
                StartCoroutine(PickUpAndDestroy());
        }

        if (moveTowards)
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position, (Time.deltaTime * 3.5f));

    }

}
