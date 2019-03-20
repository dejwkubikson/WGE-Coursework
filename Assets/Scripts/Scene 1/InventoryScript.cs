using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryScript : MonoBehaviour
{
    public Dictionary<string, int> blockDictionary; // holds the name of the block (key) and the amount player has

    void CreateInventory()
    {
        // Create a dictionary instance
        blockDictionary = new Dictionary<string, int>();

        blockDictionary.Add("Grass", 0);
        blockDictionary.Add("Dirt", 0);
        blockDictionary.Add("Stone", 0);
        blockDictionary.Add("Sand", 0);

    }

    public void AddItemToInventory(int blockType)
    {
        string blockToAdd = "";
        // Getting the block type
        switch (blockType)
        {
            case 1:
                blockToAdd = "Grass";
                break;
            case 2:
                blockToAdd = "Dirt";
                break;
            case 3:
                blockToAdd = "Sand";
                break;
            case 4:
                blockToAdd = "Stone";
                break;
            default:
                break;
        }

        blockDictionary[blockToAdd] += 1;

        //Debug.Log("Added " + blockToAdd + ", amount now: " + blockDictionary[blockToAdd]);
    }

    void SortByName()
    {

    }

    void SortByNumber()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        CreateInventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
