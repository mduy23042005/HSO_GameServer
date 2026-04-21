using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public enum TileType
{
    Ground = 0,
    Water = 1,
    Wall = 2,
    Obstacle = 3
}
public class MapView : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [SerializeField] private List<TileBase> groundTiles;
    [SerializeField] private List<TileBase> waterTiles;
    [SerializeField] private List<TileBase> wallTiles;

    private Dictionary<string, TileType> tileLookup;

    private void Start()
    {
        tileLookup = new Dictionary<string, TileType>();

        foreach (var t in groundTiles)
            tileLookup[t.name] = TileType.Ground;

        foreach (var t in waterTiles)
            tileLookup[t.name] = TileType.Water;

        foreach (var t in wallTiles)
            tileLookup[t.name] = TileType.Wall;

        groundTilemap.CompressBounds();
        waterTilemap.CompressBounds();
        wallTilemap.CompressBounds();

        BoundsInt bounds = groundTilemap.cellBounds;

        bounds.xMin = Mathf.Min(bounds.xMin, waterTilemap.cellBounds.xMin);
        bounds.yMin = Mathf.Min(bounds.yMin, waterTilemap.cellBounds.yMin);

        bounds.xMax = Mathf.Max(bounds.xMax, waterTilemap.cellBounds.xMax);
        bounds.yMax = Mathf.Max(bounds.yMax, waterTilemap.cellBounds.yMax);

        int width = bounds.size.x;
        int height = bounds.size.y;

        int[,] map = new int[width, height];

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);

                int tileCode =
                    wallTilemap.HasTile(pos) ? 2 :
                    waterTilemap.HasTile(pos) ? 1 :
                    groundTilemap.HasTile(pos) ? 0 : 0;

                map[x - bounds.xMin, y - bounds.yMin] = tileCode;
            }
        }

        ExportMapTxt(map, width, height);
        ExportMapBinary(map, width, height, bounds);
    }

    private void ExportMapTxt(int[,] map, int width, int height)
    {
        StringBuilder sb = new StringBuilder();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                sb.Append(map[x, y]);
                sb.Append(" ");
            }
            sb.AppendLine();
        }

        string dirClient = Application.streamingAssetsPath + "/Maps";
        Directory.CreateDirectory(dirClient);

        string pathClient = dirClient + $"/{SceneManager.GetActiveScene().name}_ClientDebug.txt";
        File.WriteAllText(pathClient, sb.ToString());
        Debug.Log("Txt map exported to: " + pathClient);
    }

    private void ExportMapBinary(int[,] map, int width, int height, BoundsInt bounds)
    {
        string dirClient = Application.streamingAssetsPath + "/Maps";
        Directory.CreateDirectory(dirClient);
        string pathClient = dirClient + $"/{SceneManager.GetActiveScene().name}.bin";

        string dirWebSocket = "D:/Unity project/HSO_WebSocket/Maps";
        Directory.CreateDirectory(dirWebSocket);
        string pathWebSocket = dirWebSocket + $"/{SceneManager.GetActiveScene().name}.bin";

        using (BinaryWriter writer = new BinaryWriter(File.Open(pathClient, FileMode.Create)))
        {
            writer.Write(width);
            writer.Write(height);

            writer.Write(bounds.xMin);
            writer.Write(bounds.yMin);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    writer.Write((byte)map[x, y]);
                }
            }
        }
        Debug.Log("Binary map exported: " + pathClient);

        using (BinaryWriter writer = new BinaryWriter(File.Open(pathWebSocket, FileMode.Create)))
        {
            writer.Write(width);
            writer.Write(height);

            writer.Write(bounds.xMin);
            writer.Write(bounds.yMin);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    writer.Write((byte)map[x, y]);
                }
            }
        }
        Debug.Log("Binary map exported: " + pathWebSocket);
    }
}