using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]

// This script is responsible for 'drawing' single blocks and texturing them for the voxelChunk script.
public class VoxelGenerator : MonoBehaviour {

    Mesh mesh;
    MeshCollider meshCollider;
    List<Vector3> vertexList;
    List<int> triIndexList;
    List<Vector2> UVList;
    public List<string> texNames;
    public List<Vector2> texCoords;
    public float texSize;
    public List<Vector3> cubeCenters;
    public Dictionary<string, Vector2> texNameCoordDictionary;

    int numQuads = 0;

	// Use this for initialization
	void Start () {

	}
	
    public void Initialise()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        vertexList = new List<Vector3>();
        triIndexList = new List<int>();
        UVList = new List<Vector2>();
        CreateTextureNameCoordDictionary();
    }

    public void UpdateMesh()
    {
        mesh.Clear();
        // Convert index list to array and store in mesh
        mesh.vertices = vertexList.ToArray();
        // Convert index list to array and store in mesh
        mesh.triangles = triIndexList.ToArray();
        // Convert UV list to array and store in mesh
        mesh.uv = UVList.ToArray();
        mesh.RecalculateNormals();
        // Create a collision mesh
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        ClearPreviousData();
    }

    void CreateTextureNameCoordDictionary()
    {
        // Create a dictionary instance before using
        texNameCoordDictionary = new Dictionary<string, Vector2>();

        // Check the number of names and coordiantes match
        if (texNames.Count == texCoords.Count)
        {
            // Iterate through both lists
            for(int i = 0; i < texNames.Count; i++)
            {
                // Add the pairing to the dictionary
                texNameCoordDictionary.Add(texNames[i], texCoords[i]);
            }
        }
        else
        {
            // List counts are not matching
            Debug.Log("texNames and texCoords count mismatch");
        }
    }

    public void CreateVoxel(int x, int y, int z, int textureType)
    {
        string texture;
        // Setting texture name by value
        switch (textureType)
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
                texture = "Grass";
                break;
        }

        CreateNegativeZFace(x, y, z, texture);
        CreatePositiveZFace(x, y, z, texture);

        CreateNegativeXFace(x, y, z, texture);
        CreatePositiveXFace(x, y, z, texture);

        CreateNegativeYFace(x, y, z, texture);
        CreatePositiveYFace(x, y, z, texture);
    }

    public void CreateNegativeZFace(int x, int y, int z, string texture)
    {
        Vector2 uvCoords = texNameCoordDictionary[texture];

        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x, y, z));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreatePositiveZFace(int x, int y, int z, string texture)
    {
        Vector2 uvCoords = texNameCoordDictionary[texture];

        vertexList.Add(new Vector3(x + 1, y, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreateNegativeXFace(int x, int y, int z, string texture)
    {
        Vector2 uvCoords = texNameCoordDictionary[texture];

        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x, y, z));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreatePositiveXFace(int x, int y, int z, string texture)
    {
        Vector2 uvCoords = texNameCoordDictionary[texture];

        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y, z + 1));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreateNegativeYFace(int x, int y, int z, string texture)
    {
        Vector2 uvCoords = texNameCoordDictionary[texture];

        vertexList.Add(new Vector3(x, y, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    public void CreatePositiveYFace(int x, int y, int z, string texture)
    {
        Vector2 uvCoords = texNameCoordDictionary[texture];

        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x, y + 1, z));
        AddTriangleIndices();
        AddUVCoords(uvCoords);
    }

    void AddTriangleIndices()
    {
        triIndexList.Add(numQuads * 4);
        triIndexList.Add((numQuads * 4) + 1);
        triIndexList.Add((numQuads * 4) + 3);
        triIndexList.Add((numQuads * 4) + 1);
        triIndexList.Add((numQuads * 4) + 2);
        triIndexList.Add((numQuads * 4) + 3);

        numQuads++;
    }

    void AddUVCoords(Vector2 uvCoords)
    {
        UVList.Add(new Vector2(uvCoords.x, uvCoords.y + 0.5f));
        UVList.Add(new Vector2(uvCoords.x + 0.5f, uvCoords.y + 0.5f));
        UVList.Add(new Vector2(uvCoords.x + 0.5f, uvCoords.y));
        UVList.Add(new Vector2(uvCoords.x, uvCoords.y));
    }

    // Clear previous data structures used to create the mesh
    void ClearPreviousData()
    {
        vertexList.Clear();
        triIndexList.Clear();
        UVList.Clear();
        numQuads = 0;
    }
}
