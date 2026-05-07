using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.Timeline;
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
    [SerializeField] private RawImage minimapUI;
    [SerializeField] private RawImage fullMinimapUI;
    [SerializeField] private RectTransform playerMarkerUI;
    [SerializeField] private RectTransform mobMarkerUI;
    [SerializeField] private RectTransform otherPlayerMarkerUI;
    [SerializeField] private RectTransform aStarMarkerUI;

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
    private Vector3 lastPlayerPositionFullMinimap;
    private Vector3 lastPlayerPositionMinimap;
    private RectTransform playerMarkerFullMinimap;
    private RectTransform playerMarkerMinimap;
    private List<Vector3> aStarWorldPositions = new List<Vector3>();
    private List<RectTransform> aStarMarkersMinimap = new List<RectTransform>();
    private List<RectTransform> aStarMarkersFullMinimap = new List<RectTransform>();

    private Dictionary<int, Mob> mobs;
    private Dictionary<int, RectTransform> lastMobsPositionFullMinimap = new Dictionary<int, RectTransform>();
    private Dictionary<int, RectTransform> lastMobsPositionMinimap = new Dictionary<int, RectTransform>();

    private Dictionary<int, OtherPlayer> otherPlayers;
    private Dictionary<int, RectTransform> lastOtherPlayersPositionFullMinimap = new Dictionary<int, RectTransform>();
    private Dictionary<int, RectTransform> lastOtherPlayersPositionMinimap = new Dictionary<int, RectTransform>();

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
        cachedBounds = GetMapBounds(); // mỗi map sẽ 1 scene, mỗi scene sẽ có 1 MapView riêng

        DrawFullMinimap();
        fullMinimapUI.gameObject.SetActive(false);

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
        }

        UpdateMinimapViewport();

        UpdateMarkerFullMinimap();
        UpdateMarkerMinimap();

        for (int i = 0; i < aStarMarkersMinimap.Count; i++)
        {
            if (aStarMarkersMinimap[i] == null)
                continue;

            aStarMarkersMinimap[i].anchoredPosition = WorldToMinimapPosition(aStarWorldPositions[i]);
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
    
    private void UpdateMarkerFullMinimap()
    {
        if (playerMarkerFullMinimap == null)
        {
            int size = (int)groundMinimap[0].textureRect.width;
            playerMarkerFullMinimap = Instantiate(playerMarkerUI, fullMinimapUI.transform);

            playerMarkerFullMinimap.GetComponent<Image>().color = Color.black;
            playerMarkerFullMinimap.sizeDelta = new Vector2(size, size);
            playerMarkerFullMinimap.anchorMin = new Vector2(0.5f, 0.5f);
            playerMarkerFullMinimap.anchorMax = new Vector2(0.5f, 0.5f);
            playerMarkerFullMinimap.pivot = new Vector2(0.5f, 0.5f);
        }
        if (lastPlayerPositionFullMinimap != player.transform.position)
        {
            playerMarkerFullMinimap.anchoredPosition = WorldToFullMinimapPosition(player.transform.position);

            lastPlayerPositionFullMinimap = player.transform.position;
        }

        if (mobs != null)
        {
            // tạo marker cho mob mới
            foreach (var kv in mobs)
            {
                int id = kv.Key;
                Mob mob = kv.Value;

                if (!lastMobsPositionFullMinimap.ContainsKey(id))
                {
                    int size = (int)(groundMinimap[0].textureRect.width);
                    RectTransform mobMarkerFullMinimap = Instantiate(mobMarkerUI, fullMinimapUI.transform);

                    mobMarkerFullMinimap.GetComponent<Image>().color = Color.red;
                    mobMarkerFullMinimap.sizeDelta = new Vector2(size, size);
                    mobMarkerFullMinimap.anchorMin = new Vector2(0.5f, 0.5f);
                    mobMarkerFullMinimap.anchorMax = new Vector2(0.5f, 0.5f);
                    mobMarkerFullMinimap.pivot = new Vector2(0.5f, 0.5f);

                    lastMobsPositionFullMinimap.Add(id, mobMarkerFullMinimap);
                }

                if (lastMobsPositionFullMinimap[id].anchoredPosition != WorldToFullMinimapPosition(mob.mobObject.transform.position))
                {
                    lastMobsPositionFullMinimap[id].anchoredPosition = WorldToFullMinimapPosition(mob.mobObject.transform.position);
                }
            }

            // xóa marker mob đã chết
            List<int> removeIds = new List<int>();

            foreach (var lastMobPosition in lastMobsPositionFullMinimap)
            {
                if (!mobs.ContainsKey(lastMobPosition.Key))
                {
                    Destroy(lastMobPosition.Value.gameObject);
                    removeIds.Add(lastMobPosition.Key);
                }
            }
            foreach (int id in removeIds)
            {
                lastMobsPositionFullMinimap.Remove(id);
            }
        }

        if (otherPlayers != null)
        {
            foreach (var kv in otherPlayers)
            {
                int id = kv.Key;
                OtherPlayer otherPlayer = kv.Value;

                if (!lastOtherPlayersPositionFullMinimap.ContainsKey(id))
                {
                    int size = (int)(groundMinimap[0].textureRect.width);
                    RectTransform otherPlayerMarkerFullMinimap = Instantiate(otherPlayerMarkerUI, fullMinimapUI.transform);

                    otherPlayerMarkerFullMinimap.GetComponent<Image>().color = Color.blue;
                    otherPlayerMarkerFullMinimap.sizeDelta = new Vector2(size, size);
                    otherPlayerMarkerFullMinimap.anchorMin = new Vector2(0.5f, 0.5f);
                    otherPlayerMarkerFullMinimap.anchorMax = new Vector2(0.5f, 0.5f);
                    otherPlayerMarkerFullMinimap.pivot = new Vector2(0.5f, 0.5f);

                    lastOtherPlayersPositionFullMinimap.Add(id, otherPlayerMarkerFullMinimap);
                }

                if (lastOtherPlayersPositionFullMinimap[id].anchoredPosition != WorldToFullMinimapPosition(otherPlayer.otherPlayerObject.transform.position))
                {
                    lastOtherPlayersPositionFullMinimap[id].anchoredPosition = WorldToFullMinimapPosition(otherPlayer.otherPlayerObject.transform.position);
                }
            }

            // xóa marker other player đã chết
            List<int> removeIds = new List<int>();

            foreach (var lastOtherPlayerPosition in lastOtherPlayersPositionFullMinimap)
            {
                if (!otherPlayers.ContainsKey(lastOtherPlayerPosition.Key))
                {
                    Destroy(lastOtherPlayerPosition.Value.gameObject);
                    removeIds.Add(lastOtherPlayerPosition.Key);
                }
            }

            foreach (int id in removeIds)
                lastOtherPlayersPositionFullMinimap.Remove(id);
        }
    }
    private void UpdateMarkerMinimap()
    {
        if (playerMarkerMinimap == null)
        {
            int size = (int)groundMinimap[0].textureRect.width;
            playerMarkerMinimap = Instantiate(playerMarkerUI, minimapUI.transform);

            playerMarkerMinimap.GetComponent<Image>().color = Color.black;
            playerMarkerMinimap.sizeDelta = new Vector2(size, size);
            playerMarkerMinimap.anchorMin = new Vector2(0.5f, 0.5f);
            playerMarkerMinimap.anchorMax = new Vector2(0.5f, 0.5f);
            playerMarkerMinimap.pivot = new Vector2(0.5f, 0.5f);
        }

        if (lastPlayerPositionMinimap != player.transform.position)
        {
            playerMarkerMinimap.anchoredPosition = WorldToMinimapPosition(player.transform.position);

            lastPlayerPositionMinimap = player.transform.position;
        }

        if (mobs != null)
        {
            // tạo marker cho mob mới
            foreach (var kv in mobs)
            {
                int id = kv.Key;
                Mob mob = kv.Value;

                if (!lastMobsPositionMinimap.ContainsKey(id))
                {
                    int size = (int)(groundMinimap[0].textureRect.width);
                    RectTransform mobMarkerMinimap = Instantiate(mobMarkerUI, minimapUI.transform);

                    mobMarkerMinimap.GetComponent<Image>().color = Color.red;
                    mobMarkerMinimap.sizeDelta = new Vector2(size, size);
                    mobMarkerMinimap.anchorMin = new Vector2(0.5f, 0.5f);
                    mobMarkerMinimap.anchorMax = new Vector2(0.5f, 0.5f);
                    mobMarkerMinimap.pivot = new Vector2(0.5f, 0.5f);

                    lastMobsPositionMinimap.Add(id, mobMarkerMinimap);
                }

                if (lastMobsPositionMinimap[id].anchoredPosition != WorldToMinimapPosition(mob.mobObject.transform.position))
                {
                    lastMobsPositionMinimap[id].anchoredPosition = WorldToMinimapPosition(mob.mobObject.transform.position);
                }
            }

            // xóa marker mob đã chết
            List<int> removeIds = new List<int>();

            foreach (var lastMobPosition in lastMobsPositionMinimap)
            {
                if (!mobs.ContainsKey(lastMobPosition.Key))
                {
                    Destroy(lastMobPosition.Value.gameObject);
                    removeIds.Add(lastMobPosition.Key);
                }
            }

            foreach (int id in removeIds)
            {
                lastMobsPositionMinimap.Remove(id);
            }
        }

        if (otherPlayers != null)
        {
            foreach (var kv in otherPlayers)
            {
                int id = kv.Key;
                OtherPlayer otherPlayer = kv.Value;

                if (!lastOtherPlayersPositionMinimap.ContainsKey(id))
                {
                    int size = (int)(groundMinimap[0].textureRect.width);
                    RectTransform otherPlayerMarkerMinimap = Instantiate(otherPlayerMarkerUI, minimapUI.transform);

                    otherPlayerMarkerMinimap.GetComponent<Image>().color = Color.blue;
                    otherPlayerMarkerMinimap.sizeDelta = new Vector2(size, size);
                    otherPlayerMarkerMinimap.anchorMin = new Vector2(0.5f, 0.5f);
                    otherPlayerMarkerMinimap.anchorMax = new Vector2(0.5f, 0.5f);
                    otherPlayerMarkerMinimap.pivot = new Vector2(0.5f, 0.5f);

                    lastOtherPlayersPositionMinimap.Add(id, otherPlayerMarkerMinimap);
                }

                if (lastOtherPlayersPositionMinimap[id].anchoredPosition != WorldToMinimapPosition(otherPlayer.otherPlayerObject.transform.position))
                {
                    lastOtherPlayersPositionMinimap[id].anchoredPosition = WorldToMinimapPosition(otherPlayer.otherPlayerObject.transform.position);
                }
            }

            // xóa marker mob đã chết
            List<int> removeIds = new List<int>();

            foreach (var lastOtherPlayerPosition in lastOtherPlayersPositionMinimap)
            {
                if (!otherPlayers.ContainsKey(lastOtherPlayerPosition.Key))
                {
                    Destroy(lastOtherPlayerPosition.Value.gameObject);
                    removeIds.Add(lastOtherPlayerPosition.Key);
                }
            }

            foreach (int id in removeIds)
                lastOtherPlayersPositionMinimap.Remove(id);
        }
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
    private void DrawFullMinimap()
    {
        // lấy giới hạn đường bound từng phần của map
        groundTilemap.CompressBounds();
        waterTilemap.CompressBounds();
        wallTilemap.CompressBounds();

        BoundsInt bounds = GetMapBounds();

        // lấy kích thước pixel của 1 tile minimap, tất cả tile base đề cùng kích thước nên lấy tile base đầu tiên của ground
        int tileFullMinimapWidth = (int)groundMinimap[0].textureRect.width;
        int tileFullMinimapHeight = (int)groundMinimap[0].textureRect.height;
        int borderSize = tileFullMinimapWidth;

        Texture2D texture2D = new Texture2D(bounds.size.x * tileFullMinimapWidth, bounds.size.y * tileFullMinimapHeight);

        // bắt đầu đọc theo tilemap
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int positionInt = new Vector3Int(x, y, 0);

                int drawX = (x - bounds.xMin) * tileFullMinimapWidth;
                int drawY = (y - bounds.yMin) * tileFullMinimapHeight;

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
                {
                    Debug.Log("Missing sprite for tile");
                    continue;
                }

                // bắt đầu render sprite cho minimap
                for (int pixelX = 0; pixelX < tileFullMinimapWidth; pixelX++)
                {
                    for (int pixelY = 0; pixelY < tileFullMinimapHeight; pixelY++)
                    {
                        Color c = sprite.texture.GetPixel((int)sprite.textureRect.x + pixelX, (int)sprite.textureRect.y + pixelY);
                        texture2D.SetPixel(drawX + pixelX, drawY + pixelY, c);
                    }
                }
            }
        }

        texture2D.wrapMode = TextureWrapMode.Clamp;
        texture2D.Apply();

        // gắn những gì đã render lên UI
        fullMinimapUI.texture = texture2D;
        minimapUI.texture = texture2D;

        //điều chỉnh kích thước của RawImage để hiển thị đúng kích thước minimap
        fullMinimapUI.rectTransform.sizeDelta = new Vector2(texture2D.width, texture2D.height);
    }
    private void UpdateMinimapViewport()
    {
        BoundsInt bounds = cachedBounds;

        Rect fullRect = fullMinimapUI.rectTransform.rect;

        float fullWidth = fullRect.width;
        float fullHeight = fullRect.height;

        Vector3Int playerCell = groundTilemap.WorldToCell(player.transform.position);

        float normalizedX = (playerCell.x - bounds.xMin) / (float)bounds.size.x;
        float normalizedY = (playerCell.y - bounds.yMin) / (float)bounds.size.y;

        float viewWidth = minimapUI.rectTransform.rect.width / fullWidth;
        float viewHeight = minimapUI.rectTransform.rect.height / fullHeight;

        float uvX = normalizedX - viewWidth / 2f;
        float uvY = normalizedY - viewHeight / 2f;

        uvX = Mathf.Clamp(uvX, 0f, 1f - viewWidth);
        uvY = Mathf.Clamp(uvY, 0f, 1f - viewHeight);

        minimapUI.uvRect = new Rect(uvX, uvY, viewWidth, viewHeight);
    }
    private Vector2 WorldToFullMinimapPosition(Vector3 worldPosition)
    {
        BoundsInt bounds = cachedBounds;
        Rect rect = fullMinimapUI.rectTransform.rect;
        Vector3Int cell = groundTilemap.WorldToCell(worldPosition);

        float normalizedX = (cell.x - bounds.xMin) / (float)bounds.size.x;
        float normalizedY = (cell.y - bounds.yMin) / (float)bounds.size.y;

        float uiX = normalizedX * rect.width - rect.width * 0.5f;
        float uiY = normalizedY * rect.height - rect.height * 0.5f;

        return new Vector2(uiX, uiY);
    }
    private Vector2 WorldToMinimapPosition(Vector3 worldPosition)
    {
        BoundsInt bounds = cachedBounds;
        Vector3Int cell = groundTilemap.WorldToCell(worldPosition);

        float normalizedX = (cell.x - bounds.xMin) / (float)bounds.size.x;
        float normalizedY = (cell.y - bounds.yMin) / (float)bounds.size.y;

        Rect uv = minimapUI.uvRect;
        Rect rect = minimapUI.rectTransform.rect;

        float localX = (normalizedX - uv.x) / uv.width;
        float localY = (normalizedY - uv.y) / uv.height;

        if (localX < 0 || localX > 1 || localY < 0 || localY > 1)
        {
            localX = Mathf.Clamp01(localX);
            localY = Mathf.Clamp01(localY);
        }

        float uiX = localX * rect.width - rect.width * 0.5f;
        float uiY = localY * rect.height - rect.height * 0.5f;

        return new Vector2(uiX, uiY);
    }
    public Vector3 MinimapToWorldPosition(Vector2 minimapClickPos)
    {
        BoundsInt bounds = cachedBounds;
        Rect rect = fullMinimapUI.rectTransform.rect;

        float normalizedX = (minimapClickPos.x + rect.width * 0.5f) / rect.width;
        float normalizedY = (minimapClickPos.y + rect.height * 0.5f) / rect.height;

        float worldX = normalizedX * bounds.size.x + bounds.xMin;
        float worldY = normalizedY * bounds.size.y + bounds.yMin;

        return new Vector3(worldX, worldY, 0);
    }

    public void DrawAStarPath(List<(int x, int y)> path)
    {
        ClearAStarPath();
        aStarWorldPositions.Clear();

        if (path == null || path.Count == 0)
            return;

        int size = (int)groundMinimap[0].textureRect.width;

        foreach (var node in path)
        {
            RectTransform markerFullMinimap = Instantiate(aStarMarkerUI, fullMinimapUI.transform);
            RectTransform markerMinimap = Instantiate(aStarMarkerUI, minimapUI.transform);
            Vector3 worldPos = new Vector3(node.x + 0.5f, node.y + 0.5f, 0);
            aStarWorldPositions.Add(worldPos); // cached lại những tọa độ của node trong a* path để cố định toàn bộ node khi minimap di chuyển theo player

            markerFullMinimap.GetComponent<Image>().color = Color.white;
            markerFullMinimap.sizeDelta = new Vector2(size, size);
            markerFullMinimap.anchorMin = new Vector2(0.5f, 0.5f);
            markerFullMinimap.anchorMax = new Vector2(0.5f, 0.5f);
            markerFullMinimap.pivot = new Vector2(0.5f, 0.5f);
            markerFullMinimap.anchoredPosition = WorldToFullMinimapPosition(worldPos);

            markerMinimap.GetComponent<Image>().color = Color.white;
            markerMinimap.sizeDelta = new Vector2(size, size);
            markerMinimap.anchorMin = new Vector2(0.5f, 0.5f);
            markerMinimap.anchorMax = new Vector2(0.5f, 0.5f);
            markerMinimap.pivot = new Vector2(0.5f, 0.5f);

            aStarMarkersFullMinimap.Add(markerFullMinimap);
            aStarMarkersMinimap.Add(markerMinimap);
        }
    }
    public void ClearAStarNodeMarker(int index)
    {
        if (index >= 0 && index < aStarMarkersFullMinimap.Count)
        {
            if (aStarMarkersFullMinimap[index] != null)
            {
                Destroy(aStarMarkersFullMinimap[index].gameObject);
                aStarMarkersFullMinimap[index] = null;
            }
        }
        if (index >= 0 && index < aStarMarkersMinimap.Count)
        {
            if (aStarMarkersMinimap[index] != null)
            {
                Destroy(aStarMarkersMinimap[index].gameObject);
                aStarMarkersMinimap[index] = null;
            }
        }
    }
    public void ClearAStarPath()
    {
        for (int i = 0; i < aStarMarkersFullMinimap.Count; i++)
        {
            if (aStarMarkersFullMinimap[i] != null)
            {
                Destroy(aStarMarkersFullMinimap[i].gameObject);
            }
            if (aStarMarkersMinimap[i] != null)
            {
                Destroy(aStarMarkersMinimap[i].gameObject);
            }
        }

        aStarMarkersFullMinimap.Clear();
        aStarMarkersMinimap.Clear();
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