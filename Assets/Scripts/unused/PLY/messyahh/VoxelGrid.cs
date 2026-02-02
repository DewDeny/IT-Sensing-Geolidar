using System.Collections.Generic;
using UnityEngine;

public class VoxelGrid
{
    public readonly float voxelSize;
    HashSet<Vector3Int> occupied = new();

    public VoxelGrid(float voxelSize)
    {
        this.voxelSize = voxelSize;
    }

    public void Build(PLYLoader.PointData[] points)
    {
        occupied.Clear();

        foreach (var p in points)
        {
            Vector3Int v = WorldToVoxel(p.position);
            occupied.Add(v);
        }
    }

    public bool IsOccupied(Vector3Int v)
    {
        return occupied.Contains(v);
    }

    public Vector3 VoxelCenter(Vector3Int v)
    {
        return (Vector3)v * voxelSize + Vector3.one * voxelSize * 0.5f;
    }

    Vector3Int WorldToVoxel(Vector3 p)
    {
        return new Vector3Int(
            Mathf.FloorToInt(p.x / voxelSize),
            Mathf.FloorToInt(p.y / voxelSize),
            Mathf.FloorToInt(p.z / voxelSize)
        );
    }
}