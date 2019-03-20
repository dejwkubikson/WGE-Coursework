using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableScript : MonoBehaviour
{
    GameObject player; // used to get current position
    GameObject camera; // the collectable moves to the camera so that it doesn't just move on the ground but goes to the player's face so that he clearly can see picking it up
    public int blockType; 
    private bool moveTowards = false; // if the player is close enough to the collectable it should move towards him even if he starts to move away from it

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        camera = GameObject.FindGameObjectWithTag("MainCamera");
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

            // if the object is very close then it is picked up
            if(dist < 1.0f)
            {
                InventoryScript inventory = player.GetComponent<InventoryScript>();
                inventory.AddItemToInventory(blockType);
                Destroy(this.gameObject);
            }
        }

        if (moveTowards)
            transform.position = Vector3.MoveTowards(transform.position, camera.transform.position, (Time.deltaTime * 3.5f));

    }

}
