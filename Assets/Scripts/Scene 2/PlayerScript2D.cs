using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script should be attached to the player, disables movement, starts dialogue when near the NPC.
public class PlayerScript2D : MonoBehaviour
{
    // STARTING POS X = -12, Y = 0, Z = -7.419922
    Vector3 playerPos;
    Vector3 npcPos;
    float distanceToNPC;
    PlayerMovement2D playerMovement;
    DialogueScript dialogueScript;

    // Start is called before the first frame update
    void Start()
    {
        // Getting the movement script
        playerMovement = this.gameObject.GetComponent<PlayerMovement2D>();
        playerPos = new Vector3();

        // Getting the dialogue script
        GameObject dialogueObject = GameObject.Find("DialogueObject").gameObject;
        if (dialogueObject != null)
            dialogueScript = dialogueObject.GetComponent<DialogueScript>();

        // Finding the NPC object to get it's position
        GameObject npcObject = GameObject.Find("NPC").gameObject;
        if (npcObject != null)
            npcPos = npcObject.transform.position;
        else
            Debug.Log("Couldn't find NPC!");
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = this.transform.position;
        distanceToNPC = Vector3.Distance(playerPos, npcPos);

        // If the player is close enough to the NPC and isn't in the air
        if(distanceToNPC < 3 && playerMovement._mState != MovementState.IN_AIR)
        {
            //Debug.Log("Close to the NPC.");

            // We want to start the dialogue once
            if (!(dialogueScript.dialogueStarted) && !(dialogueScript.dialogueEnded))
                dialogueScript.StartDialogue();

            // Stop movement if the dialogue hasn't ended
            if (!(dialogueScript.dialogueEnded))
                playerMovement._mState = MovementState.DISABLED;
            else playerMovement._mState = MovementState.ON_GROUND;
        }

    }
}
