using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : MonoBehaviour {

    VoxelGenerator voxelGenerator;
    InventoryScript inventoryScript;
    int[,,] terrainArray;
    int chunkSize = 16;
    public Material material;

    public List<AudioClip> soundEffects;
    public string fileName = ""; // XML file name to load chunk from

    private List<int> blockList; // Contains the amount of blocks the player has. Index 0 will store grass blocks, 1 dirt, 2 sand and 3 stones.
    // delegate signature
    public delegate void EventBlockChangedWithType(int blockType);
    
    // event instance for EventBlockChangedWithType()
    public static event EventBlockChangedWithType OnEventBlockChanged;

    void CreateCollectableBlock(int x, int y, int z, int destroyedBlock)
    {
        // Creating a cube
        GameObject collectable = GameObject.CreatePrimitive(PrimitiveType.Cube);
        // Adding rigidbody, tag, attaching script and material to it
        collectable.AddComponent<Rigidbody>();
        collectable.gameObject.tag = "Collectable";
        collectable.AddComponent<CollectableScript>();
        collectable.GetComponent<Renderer>().material = material;
        // Scaling the texture
        collectable.GetComponent<Renderer>().material.mainTextureScale = new Vector2(0.5f, 0.5f);
        // Adding force to the collectible so that it kind of jumps
        collectable.GetComponent<Rigidbody>().AddForce(collectable.transform.up * 100);

        string texture = "";
        // Getting the name of the texture from destroyed block
        switch (destroyedBlock)
        {
            case 1:
                texture = "Grass";
                break;
            case 2:
                texture = "Dirt";
                break;
            case 3:
                texture = "Sand";
                break;
            case 4:
                texture = "Stone";
                break;
            default:
                break;
        }

        // Getting the texture offset from texCoords and using it to display proper texture
        Vector2 offsetVector = voxelGenerator.texNameCoordDictionary[texture];
        collectable.GetComponent<Renderer>().material.mainTextureOffset = offsetVector;

        //collectable.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0, 0); // grass
        //collectable.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.5f, 0); // stone
        //collectable.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0, 0.5f); // dirt
        //collectable.GetComponent<Renderer>().material.mainTextureOffset = new Vector2(0.5f, 0.5f); // sand

        // Scalling the block to about one third
        collectable.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

        // Block will be placed in the middle of the destroyed block
        collectable.transform.position = new Vector3(x + 0.5f, y + 0.5f, z + 0.5f);

        // Lastly, making the variable blocktype the same as the destroyed block - used when picking the collectable up
        collectable.GetComponent<CollectableScript>().blockType = destroyedBlock;
    }

    void CreateTerrain()
    {
        // iterate horizontally on width
        for(int x = 0; x < terrainArray.GetLength(0); x++)
        {
            // iterate vertically 
            for(int y = 0; y < terrainArray.GetLength(1); y++)
            {
                // iterate per voxel horizontally on depth
                for (int z = 0; z < terrainArray.GetLength(2); z++)
                {
                    // if the voxel is not empty
                    if (terrainArray[x, y, z] != 0)
                    {
                        string tex;
                        // set texture name by value
                        switch (terrainArray[x, y, z])
                        {
                            case 1:
                                tex = "Grass";
                                break;
                            case 2:
                                tex = "Dirt";
                                break;
                            case 3:
                                tex = "Sand";
                                break;
                            case 4:
                                tex = "Stone";
                                break;
                            default:
                                tex = "Grass";
                                break;
                        }

                        // check if we need to draw the negative x face
                        if (x == 0 || terrainArray[x - 1, y, z] == 0)
                        {
                            voxelGenerator.CreateNegativeXFace(x, y, z, tex);
                        }
                        // check if we need to draw the positive x face
                        if (x == terrainArray.GetLength(0) - 1 || terrainArray[x + 1, y, z] == 0)
                        {
                            voxelGenerator.CreatePositiveXFace(x, y, z, tex);
                        }

                        // check if we need to draw the negative y face
                        if (y == 0 || terrainArray[x, y - 1, z] == 0)
                        {
                            voxelGenerator.CreateNegativeYFace(x, y, z, tex);
                        }

                        // check if we need to draw the positive y face
                        if (y == terrainArray.GetLength(1) - 1 || terrainArray[x, y + 1, z] == 0)
                        {
                            voxelGenerator.CreatePositiveYFace(x, y, z, tex);
                        }

                        // check if we need to draw the negative z face
                        if (z == 0 || terrainArray[x, y, z - 1] == 0)
                        {
                            voxelGenerator.CreateNegativeZFace(x, y, z, tex);
                        }

                        // check if we need to draw the positive z face
                        if (z == terrainArray.GetLength(2) - 1 || terrainArray[x, y, z + 1] == 0)
                        {
                            voxelGenerator.CreatePositiveZFace(x, y, z, tex);
                        }

                       // print("Create " + tex + " block.");
                    }
                }
            }
        }
    }

    public void SetBlock(Vector3 index, int blockType)
    {
        // Had to add -1 to the total length as we don't want the player to place on or destroy utmost blocks
        if ((index.x > 0 && index.x < terrainArray.GetLength(0) - 1) && (index.y > 0 && index.y < terrainArray.GetLength(1)) && (index.z > 0 && index.z < terrainArray.GetLength(2) - 1))
        {
            // Instantiate a collectable block
            if (blockType == 0)
            {
                // Saving which block was destroyed
                int destroyedBlock = terrainArray[(int)index.x, (int)index.y, (int)index.z];
                // Creating a collectable block
                CreateCollectableBlock((int)index.x, (int)index.y, (int)index.z, destroyedBlock);
            }
            else
            {
                // Update the inventory
                if (!(inventoryScript.SubtractItemFromInventory(blockType)))
                    return;
            }

            // Change the block to the required type
            terrainArray[(int)index.x, (int)index.y, (int)index.z] = blockType;
            // Create the new mesh
            CreateTerrain();
            // Update the mesh data
            voxelGenerator.UpdateMesh();

            // Play sound from list. The sound effect list is in order with the block type: 0 - destroy sound, 1 - place grass block sound, 2 - place dirt block sound, 3 - place sand block sound, 4 - place stone block sound
            GetComponent<AudioSource>().PlayOneShot(soundEffects[blockType]);

            OnEventBlockChanged(blockType);
        }
    }

    // Use this for initialization
    void Start () {
        blockList = new List<int>();

        voxelGenerator = GetComponent<VoxelGenerator>();

        inventoryScript = GameObject.Find("FPSController").GetComponent<InventoryScript>();

        // Instantiate the array with size based on chunksize;
        terrainArray = new int[chunkSize, chunkSize, chunkSize];

        voxelGenerator.Initialise();

        fileName = "AssessmentChunk2.xml"; // GET RID OF LATER!!!! @@@@@@@@@@@@@@@@@@@@@@@@@

        // Get terrainArray from XML file
        terrainArray = XMLVoxelFileWriter.LoadChunkFromXMLFile(chunkSize, fileName);

        // Draw the correct faces
        CreateTerrain();

        // Update mesh info
        voxelGenerator.UpdateMesh();
    }
	
	// Update is called once per frame
	void Update () {

    }
}
