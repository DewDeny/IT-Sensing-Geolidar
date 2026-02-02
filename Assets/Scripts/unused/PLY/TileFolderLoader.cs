using System.IO;
using UnityEngine;

public class TileFolderLoader : MonoBehaviour
{
    [Header("Folder containing .ply tiles")]
    public string tilesFolder;

    public PointTileNeo tilePrefab;

    void Start()
    {
        LoadAllTiles();
    }

    void LoadAllTiles()
    {
        if (!Directory.Exists(tilesFolder))
        {
            Debug.LogError("Tiles folder not found: " + tilesFolder);
            return;
        }

        string[] plyFiles = Directory.GetFiles(tilesFolder, "*.ply");

        Debug.Log($"Found {plyFiles.Length} ply tiles");

        foreach (string path in plyFiles)
        {
            LoadSingleTile(path);
        }
    }

    /* void LoadSingleTile(string path)
    {
        Debug.Log("Loading tile: " + Path.GetFileName(path));

        // 1. Parse PLY → vertices
        PlyVertex[] vertices = PlyLoaderNeo.Load(path);

        // 2. Create tile GameObject
        GameObject go = new GameObject(Path.GetFileNameWithoutExtension(path));
        go.transform.SetParent(transform, false);

        // 3. Build tile
        var tile = go.AddComponent<PointTileNeo>();
        tile.Build(vertices);
    }*/

    void LoadSingleTile(string path)
    {
     //   PlyVertex[] vertices = PlyLoaderNeo.Load(path);

        var tile = Instantiate(tilePrefab, transform);
        tile.name = Path.GetFileNameWithoutExtension(path);

     //   tile.Build(vertices);
    }
}