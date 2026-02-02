using System;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class PLYLoader
{
    //Data structs
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct PlyVertexRaw
    {
        public double x, y, z;
        /*    public ushort intensity;
            public byte returnNumber, numberOfReturns;
            public byte scanDirectionFlag, edgeOfFlightLine;
            public byte classification, synthetic, keypoint, withheld, overlap;
            public float scanAngleRank;
            public byte userData;
            public ushort pointSourceId;
            public double gpsTime;
            public byte scanChannel;
          */
        public ushort r, g, b;
    }

    public struct PointData
    {
        public Vector3 position;
        public Color32 color;
        public byte classification;
        public ushort intensity;
    }

    //Header reader method
    static (int count, long dataOffset) ReadHeader(FileStream fs)
    {
        using var sr = new StreamReader(
            fs,
            System.Text.Encoding.ASCII,
            false,
            1024,
            true
        );
        string line;
        int count = 0;

        while ((line = sr.ReadLine()) != null)
        {
            if (line.StartsWith("element vertex"))
                count = int.Parse(line.Split(' ')[2]);

            if (line == "end_header")
                break;
        }

        long offset = fs.Position;
        fs.Position = offset; // ensure binary read starts correctly
        return (count, offset);
    }

    //Binary load method (LoadRaw)
    /*  static PlyVertexRaw[] LoadRaw(string path)
      {
          using var fs = File.OpenRead(path);
          var (count, offset) = ReadHeader(fs);

          int stride = 57;
          int totalBytes = count * stride;

          // 1. Read binary data
          byte[] buffer = new byte[totalBytes];
          fs.Read(buffer, 0, totalBytes);

          // 2. Allocate destination array
          var raw = new PlyVertexRaw[count];

          // 3. Pin destination and copy bytes into it
          var handle = GCHandle.Alloc(raw, GCHandleType.Pinned);
          try
          {
              IntPtr dstPtr = handle.AddrOfPinnedObject();
              Marshal.Copy(buffer, 0, dstPtr, totalBytes);
          }
          finally
          {
              handle.Free();
          }

          return raw;
      } */

    //New Binary loader
    public static PointData[] LoadBinary(string path)
    {
        using var fs = File.OpenRead(path);
        using var br = new BinaryReader(fs);

        var (vertexCount, _) = ReadHeader(fs);

        PointData[] points = new PointData[vertexCount];

        for (int i = 0; i < vertexCount; i++)
        {
            double x = br.ReadDouble();
            double y = br.ReadDouble();
            double z = br.ReadDouble();

            ushort r = br.ReadUInt16();
            ushort g = br.ReadUInt16();
            ushort b = br.ReadUInt16();

            // PDAL (X,Y,Z) → Unity (X,Z,Y)
            Vector3 pos = new Vector3(
                (float)x,
                (float)z,
                (float)y
            );

            Color32 col = new Color32(
                (byte)(r >> 8),
                (byte)(g >> 8),
                (byte)(b >> 8),
                255
            );

            points[i] = new PointData
            {
                position = pos,
                color = col
            };
        }

        return points;
    }

    //Conversion method
    /*  static PointData[] Convert(
        PlyVertexRaw[] raw,
        Vector3 tileOrigin)
      {
          var outData = new PointData[raw.Length];

          for (int i = 0; i < raw.Length; i++)
          {
              ref var v = ref raw[i];

              outData[i] = new PointData
              {
                  position = new Vector3(
                      (float)(v.x - tileOrigin.x),
                      (float)(v.z - tileOrigin.z), // Z-up ? Y-up
                      (float)(v.y - tileOrigin.y)
                  ),
                  color = new Color32(
                      (byte)(v.r >> 8),
                      (byte)(v.g >> 8),
                      (byte)(v.b >> 8),
                      255
                  ),
             //     classification = v.classification,
               //   intensity = v.intensity
              };
          }

          return outData;
      }*/

    //Public entry point
    public static PointData[] Load(
      string path,
      Vector3 tileOrigin)
    {
        var points = LoadBinary(path);

        // Apply tile origin offset
        for (int i = 0; i < points.Length; i++)
        {
            points[i].position += tileOrigin;
        }

        return points;
    }
}