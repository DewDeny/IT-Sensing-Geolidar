using System.Runtime.InteropServices;
using UnityEngine;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct PlyVertex
{
    public double x;
    public double y;
    public double z;
    public ushort r;
    public ushort g;
    public ushort b;
}