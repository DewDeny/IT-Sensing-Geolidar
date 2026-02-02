using System.IO;
using System.Text;
using UnityEngine;

//Binary PLY loader (no Unity API yet)
public static class PlyLoaderNeo
{
    public struct PlyTileData
    {
        public PlyVertex[] vertices;
        public Vector3 tileOrigin;
    }

    public static PlyTileData[] Load(string path)
    {
        using var fs = File.OpenRead(path);
        using var br = new BinaryReader(fs);

        int vertexCount = ReadHeader(br);

        var vertices = new PlyVertex[vertexCount];
        for (int i = 0; i < vertexCount; i++)
        {
            vertices[i] = ReadVertex(br);
        }

        //"That code goes inside PlyLoader.Load, after all vertices are read, and before they are returned."
        //vertices were points
        Vector3 min = new Vector3(
            (float)vertices[0].x,
            (float)vertices[0].y,
            (float)vertices[0].z
        );

        for (int i = 1; i < vertices.Length; i++)
        {
            min.x = Mathf.Min(min.x, (float)vertices[i].x);
            min.y = Mathf.Min(min.y, (float)vertices[i].y);
            min.z = Mathf.Min(min.z, (float)vertices[i].z);
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].x -= min.x;
            vertices[i].y -= min.y;
            vertices[i].z -= min.z;
        }
        return null;
      /*  return new PlyTileData
        {
            vertices = vertices,
            tileOrigin = new Vector3(min.x, min.z, min.y) // Unity axis order
        };*/
    }

    static int ReadHeader(BinaryReader br)
    {
        int vertexCount = 0;

        while (true)
        {
            string line = ReadLine(br);
            if (line.StartsWith("element vertex"))
                vertexCount = int.Parse(line.Split(' ')[2]);

            if (line == "end_header")
                break;
        }

        return vertexCount;
    }

    static PlyVertex ReadVertex(BinaryReader br)
    {
        return new PlyVertex
        {
            x = br.ReadDouble(),
            y = br.ReadDouble(),
            z = br.ReadDouble(),
            r = br.ReadUInt16(),
            g = br.ReadUInt16(),
            b = br.ReadUInt16()
        };
    }

    static string ReadLine(BinaryReader br)
    {
        var sb = new StringBuilder();
        char c;
        while ((c = (char)br.ReadByte()) != '\n')
        {
            if (c != '\r') sb.Append(c);
        }
        return sb.ToString();
    }
}