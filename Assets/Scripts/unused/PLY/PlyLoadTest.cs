using System.IO;
using UnityEngine;

//Modify PlyLoadTest.cs to this:
public class PlyLoadTest : MonoBehaviour
{
    public string plyFolder;
    public Material pointMaterial;

    void Start()
    {
         var points = PlyLoaderNeo.Load(plyFolder);
          Debug.Log($"PLY loaded: {points.Length} vertices");

          //validation
          //Validate(points);

          var go = new GameObject("PLY Tile");
          go.transform.SetParent(transform, false);

          var tile = go.AddComponent<PointTileNeo>();
        //  tile.Build(points);

          var mr = go.GetComponent<MeshRenderer>();
          mr.material = pointMaterial;
        //         */
        //
        /*

        if (!Directory.Exists(plyFolder))
        {
            Debug.LogError($"PLY folder not found: {plyFolder}");
            return;
        }

        string[] files = Directory.GetFiles(plyFolder, "*.ply");

        Debug.Log($"Found {files.Length} PLY tiles");

        foreach (var path in files)
        {
            var points = PlyLoaderNeo.Load(path);

            var go = new GameObject(Path.GetFileNameWithoutExtension(path));
            go.transform.SetParent(transform, false);

            var tile = go.AddComponent<PointTileNeo>();
            tile.Build(points);

            go.GetComponent<MeshRenderer>().material = pointMaterial;
     
    }*/
    }
        }