using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.LightTransport;
using static UnityEngine.UI.Image;

//Convert PLY points → Unity Mesh
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PointTileNeo : MonoBehaviour
{
    [SerializeField]
    private Material pointMaterial;

    public float voxelSize = 0.2f;
    // Store the real-world origin(optional but recommended)
    public Vector3 worldOrigin;

    public void Build(PlyVertex[] points)
    {
        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        Vector3[] vertices = new Vector3[points.Length];
        Color32[] colors = new Color32[points.Length];
        int[] indices = new int[points.Length];
       // Vector3 origin;

        for (int i = 0; i < points.Length; i++)
        {/*
            origin = new Vector3(
                (float)points[0].x,
                (float)points[0].y,
                (float)points[0].z
            );*/

            vertices[i] = new Vector3(
                (float)points[i].x,
                (float)points[i].z,
                (float)points[i].y
            );

            //color assignment
            colors[i] = new Color32(
                (byte)(points[i].r >> 8),
                (byte)(points[i].g >> 8),
                (byte)(points[i].b >> 8),
                255
            );

            indices[i] = i;
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);
        mesh.RecalculateBounds();

        //Move the GameObject to world position
        transform.position = Vector3.zero;

        GetComponent<MeshFilter>().sharedMesh = mesh;

        //Add a material reference and assign it when building.
        var mr = GetComponent<MeshRenderer>();

        if (pointMaterial == null)
        {
            Debug.LogError("PointTile missing material reference", this);
        }
        else
        {
            mr.sharedMaterial = pointMaterial;
        }
    }
}