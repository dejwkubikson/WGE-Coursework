using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public Dictionary<string, int> blockDictionary; // holds the name of the block (key) and the amount player has
    public bool inInventoryLayer = false; // used to show up the inventory on the whole screen

    private Transform pos1;
    private Transform pos2;
    private Transform pos3;
    private Transform pos4;
    private List<Transform> posList;
    private List<string> posListType; // This will be used as the index in inventory of a certain block type 

    void CreateInventory()
    {
        // Create a dictionary instance
        blockDictionary = new Dictionary<string, int>();

        // Adding 'placeholders' to the dictionary
        blockDictionary.Add("Grass", 0);
        blockDictionary.Add("Dirt", 0);
        blockDictionary.Add("Stone", 0);
        blockDictionary.Add("Sand", 0);
    }

    // Returns a block type as a string, created this function as it's used a few times
    public string GetBlockType(int blockType)
    {
        string blockToString = "";

        switch (blockType)
        {
            case 1:
                blockToString = "Grass";
                break;
            case 2:
                blockToString = "Dirt";
                break;
            case 3:
                blockToString = "Sand";
                break;
            case 4:
                blockToString = "Stone";
                break;
            default:
                blockToString = "Grass";
                break;
        }
        return blockToString;
    }

    public void AddItemToInventory(int blockType)
    {
        // Converting the int block type to string
        string blockToAdd = GetBlockType(blockType);

        // Adding the block (value) to dictionary's key
        blockDictionary[blockToAdd] += 1;

        // If this item will appear first time in the inventory
        if (blockDictionary[blockToAdd] == 1)
        {
            int choosePos = 0;
            // Iterating through the pos list to find the first available free space
            for(int i = 0; i < posList.Count; i++)
            {
                // The first child is the image placeholder, checking if it's inactive. If not we will operate on that element as it means it's empty.
                if (!(posList[i].GetChild(0).gameObject.activeSelf))
                {
                    choosePos = i;
                    break;
                }
            }

            // Activating the image placeholder and assigning a texture to it
            posList[choosePos].GetChild(0).gameObject.SetActive(true);
            RawImage posImage = posList[choosePos].GetChild(0).gameObject.GetComponent<RawImage>();
            posImage.texture = Resources.Load<Texture2D>("Textures/" + blockToAdd + "BlockImage");

            // Activating the number of blocks text and assigning a number to it
            posList[choosePos].GetChild(1).gameObject.SetActive(true);
            Text posText = posList[choosePos].GetChild(1).gameObject.GetComponent<Text>();
            posText.text = "1";

            // This is used as the index in inventory of a certain block type
            posListType.Add(blockToAdd);
        }
        else
        {
            int posToUpdate = 0;
            // Get the possition of that block type in the posListType
            for(int i = 0; i < posListType.Count; i++)
            {
                if(posListType[i] == blockToAdd)
                {
                    posToUpdate = i;
                    break;
                }
            }

            UpdateInventory(posList[posToUpdate], posToUpdate);
        }

        //Debug.Log("Added " + blockToAdd + ", amount now: " + blockDictionary[blockToAdd]);
    }

    public void SubtractItemFromInventory(int blockType)
    {
        // Converting the int block type to string
        string blockToSubtract = GetBlockType(blockType);

        // If there isn't enough blocks
        //if (blockDictionary[blockToSubtract] < 1)
          //  return;

        blockDictionary[blockToSubtract] -= 1;

        int posToUpdate = 0;
        // Get the possition of that block type in the posListType
        for (int i = 0; i < posListType.Count; i++)
        {
            if (posListType[i] == blockToSubtract)
            {
                posToUpdate = i;
                break;
            }
        }

        UpdateInventory(posList[posToUpdate], posToUpdate);
    }

    void UpdateInventory(Transform posToUpdate, int indexToUpdate)
    {
        Text posText = posToUpdate.GetChild(1).gameObject.GetComponent<Text>();
        posText.text = blockDictionary[posListType[indexToUpdate]].ToString();
    }

    void SortByName()
    {

    }

    void SortByNumber()
    {

    }

    void SearchByName()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateInventory();

        //Getting UI elements and storing them in list
        Transform pos1 = GameObject.Find("Pos1").transform; // less 'hard code' than Transform pos1 = transform.GetChild(0); // first child element (pos1)
        Transform pos2 = GameObject.Find("Pos2").transform; // second child element (pos2)
        Transform pos3 = GameObject.Find("Pos3").transform; // third child element (pos3)
        Transform pos4 = GameObject.Find("Pos4").transform; // fourth child element (pos4)

        posList = new List<Transform>();
        posList.Add(pos1);
        posList.Add(pos2);
        posList.Add(pos3);
        posList.Add(pos4);

        posListType = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
