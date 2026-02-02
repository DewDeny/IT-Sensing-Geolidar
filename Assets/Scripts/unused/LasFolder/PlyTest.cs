using UnityEngine;

public class PlyTest : MonoBehaviour
{
    void Start()
    {
        string path = @"F:\PointClouds\TOWER_converted.ply";

        Vector3[] points = BinaryPlyLoader.Load(path);

        Debug.Log($"Loaded {points.Length} points");
        Debug.Log(points[0]);
    }
}