using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; // Using events for clicks and hovering

// This script is attached via DialogueScript to option field the player can pick. Passes it's name (dictionary key) to DialogueScript when clicked.
public class OptionScript : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DialogueScript dialogueScript;
    private string objectName; // this object's name (which is an optionID)
    private GameObject highlightImage; // the image that is active when hovering over this option to clearly show the player what he will press
    private bool clickedOnce = false; // used to prevent the function starting a few times (in case unity catches the click more than once)
    
    // Start is called before the first frame update
    void Start()
    {
        // Getting the object's name
        objectName = gameObject.name;
        // Getting the child object for hovering effect
        highlightImage = gameObject.transform.GetChild(0).gameObject;
        dialogueScript = GameObject.Find("DialogueObject").GetComponent<DialogueScript>();
    }

    // When the user clicks on this object
    public void OnPointerClick(PointerEventData eventData)
    {
        //Debug.Log("Mouse click on " + objectName);
        // If the dialogue script was found
        if (dialogueScript != null)
            // if the dialogue script has shown all the options and the user still hasn't clicked on this option
            if (dialogueScript.allOptionsShown && !(clickedOnce))
            {
                clickedOnce = true;
                // Passing the id of this option to the DialogueScript
                dialogueScript.ChooseDialogueOptionStarter(objectName);
            }
    }

    // When the user hovers over this object
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("Mouse hover over " + objectName);
        highlightImage.SetActive(true);
    }

    // When the user no longer hovers over this object
    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.SetActive(false);
    }
}
