using System.Collections.Generic;
using System.IO;
using OpenTK.Mathematics;

namespace AvaloniaStlViewer.STL;

public class StlReader
{
    /// <summary>
    /// Read stl file 
    /// </summary>
    /// <param name="filePath">stl file path</param>
    /// <returns><see cref="StlMesh"/></returns>
    public static StlMesh ReadFile(string filePath)
    {
        var mesh = new StlMesh();

        using var reader = new BinaryReader(File.OpenRead(filePath));
        reader.ReadBytes(80);   // Read Header

        uint triangleCount = reader.ReadUInt32();

        for (int i = 0; i < triangleCount; i++)
        {
            // Read normal
            var normal = new Vector3(
                reader.ReadSingle(),
                reader.ReadSingle(),
                reader.ReadSingle()
            );

            // Read vertex
            for (int j = 0; j < 3; j++)
            {
                var vertex = new Vector3(
                    reader.ReadSingle(),
                    reader.ReadSingle(),
                    reader.ReadSingle()
                );

                mesh.Vertices.Add(vertex);
                mesh.Normals.Add(normal);       // Add normal data to each vertex
            }
            
            reader.ReadUInt16();
        }
        
        return mesh;
    }
}