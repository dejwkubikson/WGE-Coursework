using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryScript : MonoBehaviour
{
    public Dictionary<string, int> blockDictionary; // holds the name of the block (key) and the amount of blocks this type the player has
    public bool inInventoryLayer = false; // used to show up the inventory on the whole screen
    public GameObject inventoryLayer;

    private Transform pos1;
    private Transform pos2;
    private Transform pos3;
    private Transform pos4;
    private List<Transform> posList;
    private bool closedInventoryLayer = false;
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

    // Returns a boolean if it is possible to place a block of this type (enough blocks in inventory)
    public bool SubtractItemFromInventory(int blockType)
    {
        // Converting the int block type to string
        string blockToSubtract = GetBlockType(blockType);

        // If there isn't enough blocks
        if (blockDictionary[blockToSubtract] < 1)
            return false;

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

        // If this was the last block in the inventory
        if(blockDictionary[blockToSubtract] == 0)
        {
            // Deactivating the image placeholder
            posList[posToUpdate].GetChild(0).gameObject.SetActive(false);

            // Deactivating the number of blocks text
            posList[posToUpdate].GetChild(1).gameObject.SetActive(false);
        }

        return true;
    }

    void UpdateInventory(Transform posToUpdate, int indexToUpdate)
    {
        Text posText = posToUpdate.GetChild(1).gameObject.GetComponent<Text>();
        posText.text = blockDictionary[posListType[indexToUpdate]].ToString();
    }

    void HighlightBlockInInventory(string block, bool highlight)
    {
        int posToModify = 0;

        // Get the possition of that block type in the posListType
        for (int i = 0; i < posListType.Count; i++)
        {
            if (posListType[i] == block)
            {
                posToModify = i;
                break;
            }
        }

        // Highlight or unhighlight block in inventory
        if (highlight)
        {
            Image posSelectedImage = posList[posToModify].GetComponent<Image>();
            posSelectedImage.color = new Color32(200, 200, 200, 255);
        }
        else
        {
            Image posImage = posList[posToModify].GetComponent<Image>();
            posImage.color = new Color32(125, 125, 125, 255);
        }
    }

    public void SearchByName(string name)
    {
        // even if the string is empty the function runs in order to unhighlight any previous highlighted blocks

        // converting the string to upper case
        name = name.ToUpper();

        // looping through dictionary
        foreach(string key in blockDictionary.Keys)
        {
            // converting the string key to uppercase
            string upperKey = key.ToUpper();
            // creating a variable that will hold the amount of similar characters
            int similarChars = 0;

            for (int i = 0; i < name.Length; i++)
            {
                // stopping loop if the string is bigger than the key 
                if (name.Length > upperKey.Length)
                    break;

                // checking the written name with the name in the dictionary
                if (name[i] == upperKey[i])
                {
                    similarChars++;
                }
            }

            if(similarChars == name.Length && similarChars > 0)
            {
                //Debug.Log("This string is similar to " + key);

                // Checking if the item is in inventory (at least one block) and highlighting it
                if (blockDictionary[key] > 0)
                    HighlightBlockInInventory(key, true);
            }
            else
                HighlightBlockInInventory(key, false);
        }
    }

    public void SortByNumberHighToLow()
    {

    }

    public void SortByNameHighToLow()
    {

    }

    public void SortByNumberLowToHigh()
    {

    }

    public void SortByNameLowToHigh()
    {

    }

    void OpenInventoryLayer()
    {
        closedInventoryLayer = false;
        // enabling the inventory layer
        inventoryLayer.SetActive(true);
    }

    void CloseInventoryLayer()
    {
        closedInventoryLayer = true;
        // disabling the inventory layer
        inventoryLayer.SetActive(false);

    }

    public int SelectFromInventory(int pos)
    {
        int blockType = 0;

        pos -= 1;

        // Changing the whole inventory to 'unselected'
        for (int i = 0; i < posList.Count; i++)
        {
            Image posImage = posList[i].GetComponent<Image>();
            posImage.color = new Color32(125, 125, 125, 255);
        }

        // Highlight current selection in inventory
        Image posSelectedImage = posList[pos].GetComponent<Image>();
        posSelectedImage.color = new Color32(200, 200, 200, 255);

        // Checking if there is any block in current selection (if the image placeholder is active)
        if (posList[pos].GetChild(0).gameObject.activeSelf)
        {
            // Check what block type is under current selection
            switch (posListType[pos])
            {
                case "Grass":
                    blockType = 1;
                    break;
                case "Dirt":
                    blockType = 2;
                    break;
                case "Sand":
                    blockType = 3;
                    break;
                case "Stone":
                    blockType = 4;
                    break;
                default:
                    break;
            }
        }
        else
            blockType = 0;

        return blockType;
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
        if (inInventoryLayer)
            OpenInventoryLayer();
        else if (!(closedInventoryLayer))
            CloseInventoryLayer();
    }
}
