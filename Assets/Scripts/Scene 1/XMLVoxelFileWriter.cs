using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XMLVoxelFileWriter{

    // Write a voxel chunk to XML file
    /*public static void SaveChunkToXMLFile(int[, ,] voxelArray, string fileName)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        // Create a write instance
        XmlWriter xmlWriter = XmlWriter.Create(fileName, writerSettings);
        // Write the beginning of the document
        xmlWriter.WriteStartDocument();

        // Create the root element
        xmlWriter.WriteStartElement("VoxelChunk");

        // Iterate through all array elements
        for (int x = 0; x < voxelArray.GetLength(0); x++)
        {
            for(int y = 0; y < voxelArray.GetLength(1); y++)
            {
                for(int z = 0; z < voxelArray.GetLength(2); z++)
                {
                    if(voxelArray[x, y, z] != 0)
                    {
                        // Create a single voxel element
                        xmlWriter.WriteStartElement("Voxel");

                        // Write an attribute to store the x, y and z index
                        xmlWriter.WriteAttributeString("x", x.ToString());
                        xmlWriter.WriteAttributeString("y", y.ToString());
                        xmlWriter.WriteAttributeString("z", z.ToString());

                        // Store the voxel type
                        xmlWriter.WriteString(voxelArray[x, y, z].ToString());

                        // End the voxel element
                        xmlWriter.WriteEndElement();
                    }
                }
            }

        }

        GameObject player = GameObject.Find("FPSController").gameObject;

        xmlWriter.WriteStartElement("PlayerData");

        xmlWriter.WriteAttributeString("xPos", player.transform.position.x.ToString());
        xmlWriter.WriteAttributeString("yPos", player.transform.position.y.ToString());
        xmlWriter.WriteAttributeString("zPos", player.transform.position.z.ToString());
        xmlWriter.WriteAttributeString("xRot", player.transform.eulerAngles.x.ToString());
        xmlWriter.WriteAttributeString("yRot", player.transform.eulerAngles.y.ToString());
        xmlWriter.WriteAttributeString("zRot", player.transform.eulerAngles.z.ToString());
        xmlWriter.WriteString("1");

        xmlWriter.WriteEndElement();

        // End the root element
        xmlWriter.WriteEndElement();
        // Write the end of the document
        xmlWriter.WriteEndDocument();
        // Close the document to save
        xmlWriter.Close();
    }
    */

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
