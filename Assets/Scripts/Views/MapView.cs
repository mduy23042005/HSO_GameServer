using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum TileType
{
    Ground = 0,
    Water = 1,
    Wall = 2,
    Obstacle = 3
}
public class MapView : MonoBehaviour, IUpdatable
{
    [SerializeField] private RawImage miniMapUI;
    [SerializeField] private RectTransform playerMarkerUI;
    [SerializeField] private RectTransform mobMarkerUI;
    [SerializeField] private RectTransform otherPlayerMarkerUI;
    [SerializeField] private RectTransform AstarMarkerUI;

    [SerializeField] private bool exportMapFile = false;
    
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap waterTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [SerializeField] private List<TileBase> groundTiles;
    [SerializeField] private List<Sprite> groundMinimap;
    [SerializeField] private List<TileBase> waterTiles;
    [SerializeField] private List<Sprite> waterMinimap;
    [SerializeField] private List<TileBase> wallTiles;
    [SerializeField] private List<Sprite> wallMinimap;

    private Dictionary<TileBase, Sprite> groundLookup;
    private Dictionary<TileBase, Sprite> waterLookup;
    private Dictionary<TileBase, Sprite> wallLookup;

    private Dictionary<string, TileType> tileLookup;
    private BoundsInt cachedBounds;

    private GameObject player;
    private Vector3 lastPlayerPosition;
    private RectTransform playerMarker;
    private List<RectTransform> aStarMarkers = new List<RectTransform>();

    private Dictionary<int, Mob> mobs;
    private Dictionary<int, RectTransform> lastMobsPosition = new Dictionary<int, RectTransform>();

    private Dictionary<int, OtherPlayer> otherPlayers;
    private Dictionary<int, RectTransform> lastOtherPlayersPosition = new Dictionary<int, RectTransform>();

    private void Awake()
    {
        groundLookup = ConvertListToMap(groundTiles, groundMinimap);
        waterLookup = ConvertListToMap(waterTiles, waterMinimap);
        wallLookup = ConvertListToMap(wallTiles, wallMinimap);
        mobs = GameObject.Find("SyncManager").GetComponent<MobsManager>().GetMobs();
        otherPlayers = GameObject.Find("SyncManager").GetComponent<SyncOtherPlayersManager>().GetOtherPlayers();
    }
    private void Start()
    {
        DrawMiniMap();
        cachedBounds = GetMapBounds(); // mỗi map sẽ 1 scene, mỗi scene sẽ có 1 MapView riêng

        if (exportMapFile)
        {
            ExportMapFile();
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.Register(this);
    }
    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.Unregister(this);
        }
    }

    public void OnUpdate()
    {
        if (player == null)
        {
            switch (LogInView.GetIDSchool())
            {
                case 1:
                    player = GameObject.Find("ChienBinh(Clone)");
                    break;
                case 2:
                    player = GameObject.Find("SatThu(Clone)");
                    break;
                case 3:
                    player = GameObject.Find("PhapSu(Clone)");
                    break;
                case 4:
                    player = GameObject.Find("XaThu(Clone)");
                    break;
            }

            if (player == null)
                return;

            if (playerMarker == null)
            {
                playerMarker = Instantiate(playerMarkerUI, miniMapUI.transform);

                playerMarker.GetComponent<Image>().color = Color.black;
                int size = (int)groundMinimap[0].textureRect.width;
                playerMarker.sizeDelta = new Vector2(size, size);
                playerMarker.anchorMin = new Vector2(.5f, .5f);
                playerMarker.anchorMax = new Vector2(.5f, .5f);
                playerMarker.pivot = new Vector2(.5f, .5f);
            }
        }
        if (lastPlayerPosition != player.transform.position)
        {
            playerMarker.anchoredPosition = WorldToMiniMapPosition(player.transform.position);

            lastPlayerPosition = player.transform.position;
        }

        if (mobs != null)
        {
            // tạo marker cho mob mới
            foreach (var kv in mobs)
            {
                int id = kv.Key;
                Mob mob = kv.Value;

                if (!lastMobsPosition.ContainsKey(id))
                {
                    RectTransform mobMarker = Instantiate(mobMarkerUI, miniMapUI.transform);

                    mobMarker.GetComponent<Image>().color = Color.red;
                    int size = (int)(groundMinimap[0].textureRect.width);
                    mobMarker.sizeDelta = new Vector2(size, size);
                    mobMarker.anchorMin = new Vector2(0.5f, 0.5f);
                    mobMarker.anchorMax = new Vector2(0.5f, 0.5f);
                    mobMarker.pivot = new Vector2(0.5f, 0.5f);

                    lastMobsPosition.Add(id, mobMarker);
                }

                if (lastMobsPosition[id].anchoredPosition != WorldToMiniMapPosition(mob.mobObject.transform.position))
                {
                    lastMobsPosition[id].anchoredPosition = WorldToMiniMapPosition(mob.mobObject.transform.position);
                }
            }

            // xóa marker mob đã chết
            List<int> removeIds = new List<int>();

            foreach (var lastMobPosition in lastMobsPosition)
            {
                if (!mobs.ContainsKey(lastMobPosition.Key))
                {
                    Destroy(lastMobPosition.Value.gameObject);
                    removeIds.Add(lastMobPosition.Key);
                }
            }

            foreach (int id in removeIds)
                lastMobsPosition.Remove(id);
        }
        if (otherPlayers != null)
        {
            foreach (var kv in otherPlayers)
            {
                int id = kv.Key;
                OtherPlayer otherPlayer = kv.Value;

                if (!lastOtherPlayersPosition.ContainsKey(id))
                {
                    RectTransform otherPlayerMarker = Instantiate(otherPlayerMarkerUI, miniMapUI.transform);

                    otherPlayerMarker.GetComponent<Image>().color = Color.blue;
                    int size = (int)(groundMinimap[0].textureRect.width);
                    otherPlayerMarker.sizeDelta = new Vector2(size, size);
                    otherPlayerMarker.anchorMin = new Vector2(0.5f, 0.5f);
                    otherPlayerMarker.anchorMax = new Vector2(0.5f, 0.5f);
                    otherPlayerMarker.pivot = new Vector2(0.5f, 0.5f);

                    lastOtherPlayersPosition.Add(id, otherPlayerMarker);
                }

                if (lastOtherPlayersPosition[id].anchoredPosition != WorldToMiniMapPosition(otherPlayer.otherPlayerObject.transform.position))
                {
                    lastOtherPlayersPosition[id].anchoredPosition = WorldToMiniMapPosition(otherPlayer.otherPlayerObject.transform.position);
                }
            }

            // xóa marker mob đã chết
            List<int> removeIds = new List<int>();

            foreach (var lastOtherPlayerPosition in lastOtherPlayersPosition)
            {
                if (!otherPlayers.ContainsKey(lastOtherPlayerPosition.Key))
                {
                    Destroy(lastOtherPlayerPosition.Value.gameObject);
                    removeIds.Add(lastOtherPlayerPosition.Key);
                }
            }

            foreach (int id in removeIds)
                lastOtherPlayersPosition.Remove(id);
        }        
    }
    public void OnLateUpdate() { }
    public void OnFixedUpdate() { }

    private Dictionary<TileBase, Sprite> ConvertListToMap(List<TileBase> tiles, List<Sprite> sprites)
    {
        Dictionary<TileBase, Sprite> map = new Dictionary<TileBase, Sprite>();

        if (tiles == null || sprites == null)
            return map;

        int count = Mathf.Min(tiles.Count, sprites.Count);

        for (int i = 0; i < count; i++)
        {
            TileBase tile = tiles[i];
            Sprite sprite = sprites[i];

            if (tile != null && !map.ContainsKey(tile))
            {
                map.Add(tile, sprite);
            }
        }

        return map;
    }

    private BoundsInt GetMapBounds()
    {
        groundTilemap.CompressBounds();
        waterTilemap.CompressBounds();
        wallTilemap.CompressBounds();

        BoundsInt bounds = groundTilemap.cellBounds;

        bounds.xMin = Mathf.Min(bounds.xMin, waterTilemap.cellBounds.xMin);
        bounds.yMin = Mathf.Min(bounds.yMin, waterTilemap.cellBounds.yMin);
        bounds.xMax = Mathf.Max(bounds.xMax, waterTilemap.cellBounds.xMax);
        bounds.yMax = Mathf.Max(bounds.yMax, waterTilemap.cellBounds.yMax);

        bounds.xMin = Mathf.Min(bounds.xMin, wallTilemap.cellBounds.xMin);
        bounds.yMin = Mathf.Min(bounds.yMin, wallTilemap.cellBounds.yMin);
        bounds.xMax = Mathf.Max(bounds.xMax, wallTilemap.cellBounds.xMax);
        bounds.yMax = Mathf.Max(bounds.yMax, wallTilemap.cellBounds.yMax);

        return bounds;
    }
    private void DrawMiniMap()
    {
        // lấy giới hạn đường bound từng phần của map
        groundTilemap.CompressBounds();
        waterTilemap.CompressBounds();
        wallTilemap.CompressBounds();

        BoundsInt bounds = GetMapBounds();

        // tăng thêm kích thước vùng render minimap để vẽ border
        // lấy kích thước pixel của 1 tile minimap, tất cả tile base đề cùng kích thước nên lấy tile base đầu tiên của ground
        int tileMinimapWidth = (int)groundMinimap[0].textureRect.width;
        int tileMinimapHeight = (int)groundMinimap[0].textureRect.height;
        int borderSize = tileMinimapWidth;

        Texture2D texture2D = new Texture2D(bounds.size.x * tileMinimapWidth + borderSize * 2, bounds.size.y * tileMinimapHeight + borderSize * 2);

        // bắt đầu đọc theo tilemap
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int positionInt = new Vector3Int(x, y, 0);

                int drawX = (x - bounds.xMin) * tileMinimapWidth + borderSize;
                int drawY = (y - bounds.yMin) * tileMinimapHeight + borderSize;

                Sprite sprite = null;

                // bắt đầu mapping tilebase sang sprite minimap
                if (wallTilemap.HasTile(positionInt))
                {
                    TileBase tile = wallTilemap.GetTile(positionInt);
                    wallLookup.TryGetValue(tile, out sprite);
                }
                else if (waterTilemap.HasTile(positionInt))
                {
                    TileBase tile = waterTilemap.GetTile(positionInt);
                    waterLookup.TryGetValue(tile, out sprite);
                }
                else if (groundTilemap.HasTile(positionInt))
                {
                    TileBase tile = groundTilemap.GetTile(positionInt);
                    groundLookup.TryGetValue(tile, out sprite);
                }

                if (sprite == null)
                    continue;

                // bắt đầu render sprite cho minimap
                for (int pixelX = 0; pixelX < tileMinimapWidth; pixelX++)
                {
                    for (int pixelY = 0; pixelY < tileMinimapHeight; pixelY++)
                    {
                        Color c = sprite.texture.GetPixel((int)sprite.textureRect.x + pixelX, (int)sprite.textureRect.y + pixelY);
                        texture2D.SetPixel(drawX + pixelX, drawY + pixelY, c);
                    }
                }
            }
        }

        texture2D.wrapMode = TextureWrapMode.Clamp;
        DrawMiniMapBorder(texture2D, texture2D.width, texture2D.height, borderSize, Color.black);
        texture2D.Apply();

        // gắn những gì đã render lên UI
        miniMapUI.texture = texture2D;

        //điều chỉnh kích thước của RawImage để hiển thị đúng kích thước minimap
        RectTransform rectTransform = miniMapUI.rectTransform;
        rectTransform.sizeDelta = new Vector2(texture2D.width, texture2D.height);
    }
    private Vector2 WorldToMiniMapPosition(Vector3 worldPosition)
    {
        BoundsInt bounds = cachedBounds;
        Rect rect = miniMapUI.rectTransform.rect;
        Vector3Int cellPosition = groundTilemap.WorldToCell(worldPosition);

        float normalizedX = (cellPosition.x - bounds.xMin) / (float)bounds.size.x;
        float normalizedY = (cellPosition.y - bounds.yMin) / (float)bounds.size.y;

        float uiX = normalizedX * rect.width - rect.width * 0.5f;
        float uiY = normalizedY * rect.height - rect.height * 0.5f;

        return new Vector2(uiX, uiY);
    }
    public Vector3 MiniMapToWorldPosition(Vector2 minimapClickPos)
    {
        BoundsInt bounds = cachedBounds;
        Rect rect = miniMapUI.rectTransform.rect;

        float normalizedX = (minimapClickPos.x + rect.width * 0.5f) / rect.width;
        float normalizedY = (minimapClickPos.y + rect.height * 0.5f) / rect.height;

        float worldX = normalizedX * bounds.size.x + bounds.xMin;
        float worldY = normalizedY * bounds.size.y + bounds.yMin;

        return new Vector3(worldX, worldY, 0);
    }

    private void DrawMiniMapBorder(Texture2D tex, int width, int height, int borderThickness, Color borderColor)
    {
        for (int b = 0; b < borderThickness; b++)
        {
            // top + bottom
            for (int x = 0; x < width; x++)
            {
                tex.SetPixel(x, b, borderColor);

                tex.SetPixel(x, height - 1 - b, borderColor);
            }

            // left + right
            for (int y = 0; y < height; y++)
            {
                tex.SetPixel(b, y, borderColor);

                tex.SetPixel(width - 1 - b, y, borderColor);
            }
        }
    }
    public void DrawAStarPath(List<(int x, int y)> path)
    {
        ClearAStarPath();

        if (path == null || path.Count == 0)
            return;

        int size = (int)groundMinimap[0].textureRect.width;

        foreach (var node in path)
        {
            RectTransform marker = Instantiate(AstarMarkerUI, miniMapUI.transform);

            marker.GetComponent<Image>().color = Color.white;
            marker.sizeDelta = new Vector2(size, size);
            marker.anchorMin = new Vector2(0.5f, 0.5f);
            marker.anchorMax = new Vector2(0.5f, 0.5f);
            marker.pivot = new Vector2(0.5f, 0.5f);
            Vector3 worldPos = new Vector3(node.x + 0.5f, node.y + 0.5f, 0);
            marker.anchoredPosition = WorldToMiniMapPosition(worldPos);

            aStarMarkers.Add(marker);
        }
    }
    public void ClearAStarNodeMarker(int index)
    {
        if (index >= 0 && index < aStarMarkers.Count)
        {
            if (aStarMarkers[index] != null)
            {
                Destroy(aStarMarkers[index].gameObject);
                aStarMarkers[index] = null;
            }
        }
    }
    public void ClearAStarPath()
    {
        foreach (var marker in aStarMarkers)
        {
            if (marker != null)
                Destroy(marker.gameObject);
        }

        aStarMarkers.Clear();
    }

    private void ExportMapFile()
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

    public void RegisterDontDestroyOnLoad() { }
}