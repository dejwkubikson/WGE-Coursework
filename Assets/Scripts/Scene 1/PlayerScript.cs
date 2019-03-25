using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour {

    public VoxelChunk voxelChunk;
    public Text playerUI;
    InventoryScript inventoryScript;

    // used to know which block the user chose to place
    private int chosenBlock = 0;

    bool ActionOnBlock(out Vector3 v, float dist, bool destroy, int blockType)
    {
        v = new Vector3();
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, dist))
        {
            // if the raycasted object is a collectable
            if (hit.collider.gameObject.tag == "Collectable")
                return false;

            // offset towards the centre of the neighbouring block
            if (destroy)
                v = hit.point + hit.normal / 2;
            else
                v = hit.point - hit.normal / 2;

            // round down to get the index of the empty;
            v.x = Mathf.Floor(v.x);
            v.y = Mathf.Floor(v.y);
            v.z = Mathf.Floor(v.z);
            return true;
        }
        return false;
    }

    void DrawCollectables()
    {
        // Find all Collectable objects
        GameObject[] collectableObjects = GameObject.FindGameObjectsWithTag("Collectable");

        foreach (GameObject collectableObject in collectableObjects)
        {
            // Making the object face the player
            collectableObject.transform.LookAt(gameObject.transform);
            // Adding force
            collectableObject.GetComponent<Rigidbody>().AddForce(collectableObject.transform.forward * 10);
        }
    }

    // Use this for initialization
    void Start () {
        //InventoryScript inventoryScript = gameObject.GetComponent<InventoryScript>();
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.Alpha1))
        {
            InventoryScript inventoryScript = gameObject.GetComponent<InventoryScript>();
            chosenBlock = inventoryScript.SelectFromInventory(1);
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            InventoryScript inventoryScript = gameObject.GetComponent<InventoryScript>();
            chosenBlock = inventoryScript.SelectFromInventory(2);
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            InventoryScript inventoryScript = gameObject.GetComponent<InventoryScript>();
            chosenBlock = inventoryScript.SelectFromInventory(3);
        }
        if (Input.GetKey(KeyCode.Alpha4))
        {
            InventoryScript inventoryScript = gameObject.GetComponent<InventoryScript>();
            chosenBlock = inventoryScript.SelectFromInventory(4);
        }


        playerUI.text = "Chosen block: " + chosenBlock;

        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 v;
			if(ActionOnBlock(out v, 4, false, 0))
            {
                voxelChunk.SetBlock(v, 0);
            }
        }

        if (Input.GetButtonDown("Fire2"))
        {
            Vector3 v;
			if (ActionOnBlock(out v, 4, true, chosenBlock))
            {
                Debug.Log(v);
                voxelChunk.SetBlock(v, chosenBlock);
            }
        }

        if (Input.GetKey(KeyCode.E))
            DrawCollectables();

	}
}
