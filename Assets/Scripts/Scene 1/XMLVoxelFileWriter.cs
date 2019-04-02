using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

// This script reads voxel chunk from XML file
public class XMLVoxelFileWriter{

    // Read a voxel chunk from XML file
    public static int[, ,] LoadChunkFromXMLFile(int size, string fileName)
    {
        int[, ,] voxelArray = new int[size, size, size];

        // Create ab XML reader with the file supplied
        XmlReader xmlReader = XmlReader.Create(fileName);

        // Iterate through and read every line in the XML file
        while(xmlReader.Read() && System.IO.File.Exists(fileName))
        {
            if(xmlReader.IsStartElement("Voxel"))
            {
                // Retrieve x, y and z attributes and store as int
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);

                xmlReader.Read(); // Goes to another node!

                int value = int.Parse(xmlReader.Value);

                voxelArray[x, y, z] = value;
            }
        }
        return voxelArray;
    }
}
