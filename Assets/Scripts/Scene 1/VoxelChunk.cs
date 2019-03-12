using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelChunk : MonoBehaviour {

    VoxelGenerator voxelGenerator;
    int[,,] terrainArray;
    int chunkSize = 16;

    public List<AudioClip> soundEffects;
    public string fileName = ""; // XML file name to load chunk from

    private List<int> blockList; // Contains the amount of blocks the player has. Index 0 will store grass blocks, 1 dirt, 2 sand and 3 stones.
    // delegate signature
    public delegate void EventBlockChangedWithType(int blockType);
    
    // event instance for EventBlockChangedWithType()
    public static event EventBlockChangedWithType OnEventBlockChanged;

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

                        //voxelGenerator.CreateVoxel(x, y, z, tex);
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
                // Saving which block was destroyed and which one should be created as collectible
                int destroyedBlock = terrainArray[(int)index.x, (int)index.y, (int)index.z];

                // Creating the block and placing it in the same position @@@@@@@@@@@@@@@@@@@@@@@@@
                voxelGenerator.CreateVoxel2((int)index.x + 0.3f, (int)index.y, (int)index.z + 0.3f, destroyedBlock);
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
