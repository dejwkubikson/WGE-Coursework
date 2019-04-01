using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson; // used to disable and enable FirstPersonController script

public class InventoryScript : MonoBehaviour
{
    public Dictionary<string, int> blockDictionary; // holds the name of the block (key) and the amount of blocks this type the player has
    public bool inInventoryLayer = false; // used to show up the inventory on the whole screen
    public GameObject inventoryLayer;
    public InputField inputField;

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

    // Clears the whole inventory
    void ClearInventory()
    {
        // Iterating through the pos game objects
        for (int i = 0; i < posList.Count; i++)
        {
            // If the object is active disabling it and its components
            if (posList[i].GetChild(0).gameObject.activeSelf)
            {
                posList[i].GetChild(0).gameObject.SetActive(false);
                posList[i].GetChild(1).gameObject.SetActive(false);
                Text posText = posList[i].GetChild(1).gameObject.GetComponent<Text>();
                posText.text = "0";
            }
        }
    }

    // Adds a block to the inventory
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
            for (int i = 0; i < posList.Count; i++)
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

            // Counting the amount of blocks the player has (at least 1)
            int amountOfBlocks = 0;
            foreach (int value in blockDictionary.Values)
            {
                if (value > 0)
                    amountOfBlocks++;
            }

            // Checking if the posListType length is bigger than the amount of blocks that player holds
            if (posListType.Count >= amountOfBlocks)
            {
                // Loking for first empty space that was reserved 
                for(int i = 0; i < posListType.Count; i++)
                {
                    if (posListType[i] == "")
                        posListType[i] = blockToAdd;
                }
            }
            else
                // This is used as the index in inventory of a certain block type
                posListType.Add(blockToAdd);
        }
        else
        {
            int posToUpdate = 0;
            // Get the possition of that block type in the posListType
            for (int i = 0; i < posListType.Count; i++)
            {
                if (posListType[i] == blockToAdd)
                {
                    posToUpdate = i;
                    break;
                }
            }

            UpdateInventory(posList[posToUpdate], blockToAdd);
        }

        // Debug.Log("Added " + blockToAdd + ", amount now: " + blockDictionary[blockToAdd]);
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
        // Get the position of that block type in the posListType
        for (int i = 0; i < posListType.Count; i++)
        {
            if (posListType[i] == blockToSubtract)
            {
                posToUpdate = i;
                break;
            }
        }

        UpdateInventory(posList[posToUpdate], blockToSubtract);

        // If this was the last block in the inventory
        if (blockDictionary[blockToSubtract] == 0)
        {
            // Deactivating the image placeholder
            posList[posToUpdate].GetChild(0).gameObject.SetActive(false);
            // Deactivating the number of blocks text
            posList[posToUpdate].GetChild(1).gameObject.SetActive(false);

            // Changing the block type to empty string to reserve this space for later use (there still may be blocks in inventory 'after' this one and there needs to be access to them)
            for(int i = 0; i < posListType.Count; i++)
            {
                if (posListType[i] == blockToSubtract)
                    posListType[i] = "";
            }
        }

        return true;
    }

    // Updates inventory (text amount of blocks)
    void UpdateInventory(Transform posToUpdate, string blockToUpdate)
    {
        Text posText = posToUpdate.GetChild(1).gameObject.GetComponent<Text>();
        posText.text = blockDictionary[blockToUpdate].ToString();
    }

    // Highlights block in inventory (used in search by name only)
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

    // Searches through the inventory for certain block name
    public void SearchByName(string name)
    {
        // Even if the string is empty the function runs in order to unhighlight any previous highlighted blocks
        // Converting the string to upper case to make it easier for the user to search through the inventory
        name = name.ToUpper();

        // If there is only one block highlighted, it will be selected for the user
        int highlightedObjects = 0;
        int highlightedPos = 0;

        int dictionaryIndex = 0;
        // Looping through dictionary
        foreach (string key in blockDictionary.Keys)
        {
            // Converting the string key to uppercase
            string upperKey = key.ToUpper();
            // Creating a variable that will hold the amount of similar characters
            int similarChars = 0;

            for (int i = 0; i < name.Length; i++)
            {
                // Stopping loop if the string is bigger than the key 
                if (name.Length > upperKey.Length)
                    break;

                // Checking the written name with the name in the dictionary
                if (name[i] == upperKey[i])
                {
                    similarChars++;
                }
            }

            if (similarChars == name.Length && similarChars > 0)
            {
                //Debug.Log("This string is similar to " + key);

                // Checking if the item is in inventory (at least one block) and highlighting it
                if (blockDictionary[key] > 0)
                {
                    HighlightBlockInInventory(key, true);
                    highlightedObjects++;
                    highlightedPos = dictionaryIndex;
                }
            }
            else
                HighlightBlockInInventory(key, false);

            dictionaryIndex++;
        }

        // If only one object was highlighted throughout the name search it will be selected for the player
        if (highlightedObjects == 1)
        {
            gameObject.GetComponent<PlayerScript>().chosenBlock = SelectFromInventory(highlightedPos);
        }
    }

    // Divides the integer list
    private static List<int> MergeSort(List<int> toSort)
    {
        // If there is none or only one element the list is already sorted (a list of 1 element is considered sorted)
        if (toSort.Count <= 1)
            return toSort;

        // Creating left and right lists to divide the main list into sublists
        List<int> leftSide = new List<int>();
        List<int> rightSide = new List<int>();

        // Finding the central point of the list to sort
        int center = toSort.Count / 2;

        // Dividing the unsorted lists and adding elements from 0 to central index to left list
        for (int i = 0; i < center; i++)
        {
            leftSide.Add(toSort[i]);
        }

        // Adding elements from central to last index to right list
        for (int i = center; i < toSort.Count; i++)
        {
            rightSide.Add(toSort[i]);
        }

        // Performing the action until there will be only one element in the list
        leftSide = MergeSort(leftSide);
        rightSide = MergeSort(rightSide);

        // Merging the lists
        return Merge(leftSide, rightSide);
    }

    // Sorts and merges divided integer lists
    private static List<int> Merge(List<int> leftSide, List<int> rightSide)
    {
        // Creating a list that will contain the result of merging
        List<int> sortedList = new List<int>();

        while (leftSide.Count > 0 || rightSide.Count > 0)
        {
            if (leftSide.Count > 0 && rightSide.Count > 0)
            {
                // Comparing first elements in both lists to see which is smaller. Adding the element to the result list and removing it from the list that contained the smaller value
                if (leftSide[0] <= rightSide[0])
                {
                    sortedList.Add(leftSide[0]);
                    leftSide.Remove(leftSide[0]);
                }
                else
                {
                    sortedList.Add(rightSide[0]);
                    rightSide.Remove(rightSide[0]);
                }
            }
            // If one of the lists is empty the element from the other one is the smaller one
            else if (leftSide.Count > 0)
            {
                sortedList.Add(leftSide[0]);
                leftSide.Remove(leftSide[0]);
            }
            else if (rightSide.Count > 0)
            {
                sortedList.Add(rightSide[0]);
                rightSide.Remove(rightSide[0]);
            }
        }
        return sortedList;
    }

    // Divides the string list
    private static List<string> MergeStringSort(List<string> toSort)
    {
        // If there is none or only one element the list is already sorted (a list of 1 element is considered sorted)
        if (toSort.Count <= 1)
            return toSort;

        // Creating left and right lists to divide the main list into sublists
        List<string> leftSide = new List<string>();
        List<string> rightSide = new List<string>();

        // Finding the central point of the list to sort
        int center = toSort.Count / 2;

        // Dividing the unsorted lists and adding elements from 0 to central index to left list
        for (int i = 0; i < center; i++)
        {
            leftSide.Add(toSort[i]);
        }

        // Adding elements from central to last index to right list
        for (int i = center; i < toSort.Count; i++)
        {
            rightSide.Add(toSort[i]);
        }

        // Performing the action until there will be only one element in the list
        leftSide = MergeStringSort(leftSide);
        rightSide = MergeStringSort(rightSide);

        // Merging the lists
        return MergeString(leftSide, rightSide);
    }

    // Sorts and merges divided integer lists
    private static List<string> MergeString(List<string> leftSide, List<string> rightSide)
    {
        // Creating a list that will contain the result of merging
        List<string> sortedList = new List<string>();

        while (leftSide.Count > 0 || rightSide.Count > 0)
        {
            if (leftSide.Count > 0 && rightSide.Count > 0)
            {
                // Comparing first elements in both lists to see which is smaller. Adding the element to the result list and removing it from the list that contained the smaller value
                if (string.Compare(leftSide[0], rightSide[0]) <= 0)
                {
                    sortedList.Add(leftSide[0]);
                    leftSide.Remove(leftSide[0]);
                }
                else
                {
                    sortedList.Add(rightSide[0]);
                    rightSide.Remove(rightSide[0]);
                }
            }
            // If one of the lists is empty the element from the other one is the smaller one
            else if (leftSide.Count > 0)
            {
                sortedList.Add(leftSide[0]);
                leftSide.Remove(leftSide[0]);
            }
            else if (rightSide.Count > 0)
            {
                sortedList.Add(rightSide[0]);
                rightSide.Remove(rightSide[0]);
            }
        }
        return sortedList;
    }

    // Sorting by amount of blocks from high to low
    public void SortByNumberHighToLow()
    {
        Debug.Log("Function SortByNumberHighToLow()");

        // This list will contain the amount of blocks the user has
        List<int> unsorted = new List<int>();

        // This list will contain sorted amount of blocks the user has
        List<int> sortedList = new List<int>();

        // Assigning values to unsorted list from the dictionary, adding only values that are greater than 0
        foreach (int value in blockDictionary.Values)
        {
            if(value > 0)
                unsorted.Add(value);
        }

        sortedList = MergeSort(unsorted);
        // Reverting the list as the sorting is done from low to high
        sortedList.Reverse();

        // Clearing posListyType and the inventory as they will be redone
        posListType.Clear();
        ClearInventory();

        // Comparing the values with the dictionary assigning and changing the position of blocks in inventory
        for (int i = 0; i < sortedList.Count; i++)
        {
            bool foundSameKey = false;
            string blockType = "";

            foreach (string key in blockDictionary.Keys)
            {
                // Checking if the key has been added to the posListType - eliminating issues when blocks have same value
                for (int j = 0; j < posListType.Count; j++)
                {
                    if (posListType[j] == key)
                    {
                        // Debug.Log("The " + key + " is already in posListType");
                        foundSameKey = true;
                    }
                    else
                        foundSameKey = false;
                }

                // If the key has the same value as the value in the sorted list
                if (blockDictionary[key].Equals(sortedList[i]) && sortedList[i] != 0 && !(foundSameKey))
                {
                    Debug.Log("Found " + key);
                    blockType = key;
                    break;
                }
            }

            if (!(foundSameKey))
            {
                ChangePlaceInInventory(i, blockType);
                posListType.Add(blockType);
            }
        }

        // Selecting the first block in the sorted inventory
        gameObject.GetComponent<PlayerScript>().chosenBlock = SelectFromInventory(1);
    }

    // Sorting by amount of block from low to high
    public void SortByNumberLowToHigh()
    {
        Debug.Log("Function SortByNumberLowToHigh()");

        // This list will contain the amount of blocks the user has
        List<int> unsorted = new List<int>();

        // This list will contain sorted amount of blocks the user has
        List<int> sortedList = new List<int>();

        // Assigning values to unsorted list from the dictionary, adding only values that are greater than 0
        foreach (int value in blockDictionary.Values)
        {
            if(value > 0)
                unsorted.Add(value);
        }

        sortedList = MergeSort(unsorted);

        // Clearing posListyType and the inventory as they will be redone
        posListType.Clear();
        ClearInventory();

        // Comparing the values with the dictionary assigning and changing the position of blocks in inventory
        for (int i = 0; i < sortedList.Count; i++)
        {
            bool foundSameKey = false;
            string blockType = "";

            foreach (string key in blockDictionary.Keys)
            {
                // Checking if the key has been added to the posListType - eliminating issues when blocks have same value
                for (int j = 0; j < posListType.Count; j++)
                {
                    if (posListType[j] == key)
                    {
                        // Debug.Log("The " + key + " is already in posListType");
                        foundSameKey = true;
                    }
                    else
                        foundSameKey = false;
                }

                 // If the key has the same value as the value in the sorted list
                 if (blockDictionary[key].Equals(sortedList[i]) && !(foundSameKey))
                 {
                    Debug.Log("Found " + key);
                    blockType = key;
                    break;
                 }
            }

             // If the value of the block is 0 then the blocks will be shifted to the left (a block with 0 amount shouldn't be shown in the inventory and shouldn't take space as well)
             if (blockType != "" && !(foundSameKey))
             {
                 ChangePlaceInInventory(i, blockType);
                 posListType.Add(blockType);
             }
        }

        // Selecting the first block in the sorted inventory
        gameObject.GetComponent<PlayerScript>().chosenBlock = SelectFromInventory(1);
    }

    // Sorting by block name from low to high
    public void SortByNameLowToHigh()
    {
        Debug.Log("Function SortByNameLowToHigh()");

        // This list will contain the names of blocks the user has
        List<string> unsorted = new List<string>();

        // This list will contain sorted amount of blocks the user has
        List<string> sortedList = new List<string>();

        // Assigning values to unsorted list from the dictionary, adding only keys that have a value of at least 1 block
        foreach (string key in blockDictionary.Keys)
        {
            // If the player has at lest one block of this kind
            if(!(blockDictionary[key].Equals(0)))
                unsorted.Add(key);
        }

        sortedList = MergeStringSort(unsorted);

        // Clearing posListyType and the inventory as they will be redone
        posListType.Clear();
        ClearInventory();

        // Comparing the keys with the dictionary, assigning and changing the position of blocks in inventory
        for (int i = 0; i < sortedList.Count; i++)
        {
            string blockType = "";

            foreach (string key in blockDictionary.Keys)
            {
                // If the key is found in the sorted list the same value as the value in the sorted list
                if (sortedList[i] == key)
                {
                    Debug.Log("Found " + key);
                    blockType = key;
                    break;
                }
            }

            if (blockType != "")
            {
                Debug.Log("Adding " + blockType + " to pos " + i);
                ChangePlaceInInventory(i, blockType);
                posListType.Add(blockType);
            }
            else
                posListType.Add("");
        }

        for(int i = 0; i < posListType.Count; i++)
        {
            Debug.Log("BLOCK " + posListType[i] + " AT POS " + i);
        }

        // Selecting the first block in the sorted inventory
        gameObject.GetComponent<PlayerScript>().chosenBlock = SelectFromInventory(1);
    }

    // Sorting by block name from high to low
    public void SortByNameHighToLow()
    {
        Debug.Log("Function SortByNameHighToLow()");

        // This list will contain the names of blocks the user has
        List<string> unsorted = new List<string>();

        // This list will contain sorted amount of blocks the user has
        List<string> sortedList = new List<string>();

        // Assigning values to unsorted list from the dictionary, adding only keys that have a value of at least 1 block
        foreach (string key in blockDictionary.Keys)
        {
            // If the player has at lest one block of this kind
            if (!(blockDictionary[key].Equals(0)))
                unsorted.Add(key);
        }

        sortedList = MergeStringSort(unsorted);
        // Reverting the list as the sorting is done from low to high
        sortedList.Reverse();

        // Clearing posListyType and the inventory as they will be redone
        posListType.Clear();
        ClearInventory();

        // Comparing the keys with the dictionary, assigning and changing the position of blocks in inventory
        for (int i = 0; i < sortedList.Count; i++)
        {
            string blockType = "";

            foreach (string key in blockDictionary.Keys)
            {
                // If the key is found in the sorted list the same value as the value in the sorted list
                if (sortedList[i] == key)
                {
                    Debug.Log("Found " + key);
                    blockType = key;
                    break;
                }
            }

            if (blockType != "")
            {
                Debug.Log("Adding " + blockType + " to pos " + i);
                ChangePlaceInInventory(i, blockType);
                posListType.Add(blockType);
            }
            else
                posListType.Add("");
        }

        for (int i = 0; i < posListType.Count; i++)
        {
            Debug.Log("BLOCK " + posListType[i] + " AT POS " + i);
        }

        // Selecting the first block in the sorted inventory
        gameObject.GetComponent<PlayerScript>().chosenBlock = SelectFromInventory(1);

    }

    // Opens the inventory layer with the buttons
    void OpenInventoryLayer()
    {
        closedInventoryLayer = false;
        // enabling the inventory layer
        inventoryLayer.SetActive(true);
    }

    // Closes the inventory layer
    void CloseInventoryLayer()
    {
        closedInventoryLayer = true;
        // disabling the inventory layer
        inventoryLayer.SetActive(false);

    }

    // Changes place of block in the inventory, used while sorting
    void ChangePlaceInInventory(int changeTo, string blockType)
    {
        posList[changeTo].GetChild(0).gameObject.SetActive(true);
        RawImage posImage = posList[changeTo].GetChild(0).gameObject.GetComponent<RawImage>();
        posImage.texture = Resources.Load<Texture2D>("Textures/" + blockType + "BlockImage");

        posList[changeTo].GetChild(1).gameObject.SetActive(true);
        Text posText = posList[changeTo].GetChild(1).gameObject.GetComponent<Text>();
        posText.text = blockDictionary[blockType].ToString();
    }

    // Select a block from the inventory
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
        Transform pos1 = GameObject.Find("Pos1").transform; // first inventory element (Pos1)
        Transform pos2 = GameObject.Find("Pos2").transform; // second inventory element (Pos2)
        Transform pos3 = GameObject.Find("Pos3").transform; // third inventory element (Pos3)
        Transform pos4 = GameObject.Find("Pos4").transform; // fourth inventory element (Pos4)

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
        if (Input.GetKeyDown(KeyCode.E))
        {
            // If the player is in inventory layer and has pressed on the input field the inventory layer will not disappear when he types in 'E' - e.g when looking for a stone
            if (inInventoryLayer && inputField.isFocused == false)
            {
                inInventoryLayer = false;
                // Activating first person controller script so that the player can move
                gameObject.GetComponent<FirstPersonController>().enabled = true;
                // Making the cursor invisible and locking it
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                inInventoryLayer = true;
                // Deactivating first person controller script so that the player doesn't move around
                gameObject.GetComponent<FirstPersonController>().enabled = false;
                // Making the cursor visible and unlocking it
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }

        if (inInventoryLayer)
            OpenInventoryLayer();
        else if (!(closedInventoryLayer))
            CloseInventoryLayer();
    }
}
