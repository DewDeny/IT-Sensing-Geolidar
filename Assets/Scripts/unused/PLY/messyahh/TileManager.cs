using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Tile settings")]
    public float tileSize = 50f;
    public int loadRadius = 1; // tiles around camera

    [Header("Data")]
    [SerializeField]
    public string tileFolder = "C:/PointClouds/PointCloudTiles"; 

    Dictionary<Vector2Int, Tile> loadedTiles = new();

    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        var center = WorldToTile(cam.transform.position);
        var needed = ComputeNeededTiles(center);

        // Load new tiles
        foreach (var id in needed)
        {
            if (!loadedTiles.ContainsKey(id))
               Debug.Log($"Scheduling tile {id}");
            _ = LoadTileAsync(id);
        }

        // Unload far tiles
        var toRemove = new List<Vector2Int>();
        foreach (var kv in loadedTiles)
        {
            if (!needed.Contains(kv.Key))
                toRemove.Add(kv.Key);
        }

        foreach (var id in toRemove)
            UnloadTile(id);

        //Hook it to mouse click (debug visualization)
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (RaycastVoxels(ray, 200f, out var hit))
            {
                Debug.DrawRay(hit, Vector3.up, Color.red, 2f);
                Debug.Log($"Voxel hit at {hit}");
            }
        }
    }

    Vector2Int WorldToTile(Vector3 worldPos)
    {
        return new Vector2Int(
            Mathf.FloorToInt(worldPos.x / tileSize),
            Mathf.FloorToInt(worldPos.z / tileSize)
        );
    }

    HashSet<Vector2Int> ComputeNeededTiles(Vector2Int center)
    {
        var set = new HashSet<Vector2Int>();

        for (int y = -loadRadius; y <= loadRadius; y++)
            for (int x = -loadRadius; x <= loadRadius; x++)
                set.Add(new Vector2Int(center.x + x, center.y + y));

        return set;
    }

    async Task LoadTileAsync(Vector2Int id)
    {
        string path = $"{tileFolder}/tile_{id.x}_{id.y}.ply";
        if (!System.IO.File.Exists(path))
            return;

        // Prevent double load
        loadedTiles[id] = null;

        await Task.Run(() =>
        {
            Vector3 origin = new Vector3(
                id.x * tileSize,
                0,
                id.y * tileSize
            );

            var points = PLYLoader.Load(path, origin);

            //////
            //Showing what and where the tiles are loaded
            Debug.Log($"Loaded tile {id.x},{id.y} ({points.Length} points)");

            var p0 = points[0].position;
            Debug.Log($"Tile {id}: first point at {p0}");
          
            float minY = float.MaxValue, maxY = float.MinValue;
            float minZ = float.MaxValue, maxZ = float.MinValue;

            foreach (var p in points)
            {
                minY = Mathf.Min(minY, p.position.y);
                maxY = Mathf.Max(maxY, p.position.y);
                minZ = Mathf.Min(minZ, p.position.z);
                maxZ = Mathf.Max(maxZ, p.position.z);
            }

            Debug.Log($"Y range: {minY} to {maxY}, Z range: {minZ} to {maxZ}");
            ///////

            lock (loadedTiles)
            {
                loadedTiles[id] = new Tile
                {
                    id = id,
                    points = points
                };
            }

            //Build voxels when a tile loads
            var tile = new Tile
            {
                id = id,
                points = points
            };

            tile.BuildVoxelGrid(0.2f);

            lock (loadedTiles)
            {
                loadedTiles[id] = tile;
            }
        });

    }

    void UnloadTile(Vector2Int id)
    {
        if (!loadedTiles.TryGetValue(id, out var tile))
            return;

        tile?.Dispose();
        loadedTiles.Remove(id);
    }

    //Raycast through voxels
    bool RaycastVoxels(Ray ray, float maxDistance, out Vector3 hitPos)
    {
        float t = 0f;
        float step = 0.1f; // smaller than voxelSize

        while (t < maxDistance)
        {
            Vector3 p = ray.origin + ray.direction * t;
            Vector2Int tileId = WorldToTile(p);

            if (loadedTiles.TryGetValue(tileId, out var tile) &&
                tile?.voxelGrid != null)
            {
                Vector3 local = p - new Vector3(
                    tileId.x * tileSize,
                    0,
                    tileId.y * tileSize
                );

                Vector3Int v = new Vector3Int(
                    Mathf.FloorToInt(local.x / 0.2f),
                    Mathf.FloorToInt(local.y / 0.2f),
                    Mathf.FloorToInt(local.z / 0.2f)
                );

                if (tile.voxelGrid.IsOccupied(v))
                {
                    hitPos = tile.voxelGrid.VoxelCenter(v)
                           + new Vector3(tileId.x * tileSize, 0, tileId.y * tileSize);
                    return true;
                }
            }

            t += step;
        }

        hitPos = default;
        return false;
    }
}

public class Tile
{
    public Vector2Int id;
    public PLYLoader.PointData[] points;
    public VoxelGrid voxelGrid;

    public void BuildVoxelGrid(float voxelSize)
    {
        voxelGrid = new VoxelGrid(voxelSize);
        voxelGrid.Build(points);
    }

    public void Dispose()
    {
        points = null;
        voxelGrid = null;
    }
}