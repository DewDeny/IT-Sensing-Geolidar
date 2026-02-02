using System;
using System.IO;
using System.Text;
using UnityEngine;

public static class BinaryPlyLoader
{
    public static Vector3[] Load(string path)
    {
        using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        using var reader = new BinaryReader(fs);

        // 1. Read header as ASCII
        int vertexCount = 0;
        bool headerEnded = false;

        while (!headerEnded)
        {
            string line = ReadLine(reader);

            if (line.StartsWith("element vertex"))
            {
                var parts = line.Split(' ');
                vertexCount = int.Parse(parts[2]);
            }
            else if (line == "end_header")
            {
                headerEnded = true;
            }
        }

        if (vertexCount == 0)
            throw new Exception("PLY contains no vertices.");

        // 2. Read binary vertex data
        Vector3[] points = new Vector3[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            float x = reader.ReadSingle();
            float y = reader.ReadSingle();
            float z = reader.ReadSingle();

            // CloudCompare is Z-up → Unity Y-up
            points[i] = new Vector3(x, z, y);
        }

        return points;
    }

    private static string ReadLine(BinaryReader reader)
    {
        var bytes = new System.Collections.Generic.List<byte>();
        byte b;

        while ((b = reader.ReadByte()) != '\n')
        {
            if (b != '\r')
                bytes.Add(b);
        }

        return Encoding.ASCII.GetString(bytes.ToArray());
    }
}