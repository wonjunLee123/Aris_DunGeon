using UnityEngine;
using System.Collections.Generic;

public class DungeonGenerator2D : MonoBehaviour
{
    [Header("Room Settings")]
    public int roomCount = 6;
    public Vector2 roomSizeMin = new Vector2(3, 3);
    public Vector2 roomSizeMax = new Vector2(5, 5);
    public float wallThickness = 0.3f;
    public Color wallColor = Color.yellow;

    [Header("Corridor Settings")]
    public float corridorWidth = 1.2f;
    public Color corridorColor = Color.gray;

    [Header("Layer/Tag Settings")]
    public string wallTag = "Wall";
    public string wallLayer = "Wall";

    [Header("Map Settings")]
    public Vector2 mapSize = new Vector2(30, 20);
    public int maxPlacementAttempts = 100;

    private List<Rect> rooms = new List<Rect>();
    private List<Vector2> centers = new List<Vector2>();

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        rooms.Clear();
        centers.Clear();

        int placed = 0;
        int attempts = 0;

        // === 1. 겹치지 않게 방 생성 ===
        while (placed < roomCount && attempts < maxPlacementAttempts)
        {
            attempts++;

            Vector2 size = new Vector2(
                Random.Range(roomSizeMin.x, roomSizeMax.x),
                Random.Range(roomSizeMin.y, roomSizeMax.y)
            );

            Vector2 pos = new Vector2(
                Random.Range(-mapSize.x / 2f, mapSize.x / 2f),
                Random.Range(-mapSize.y / 2f, mapSize.y / 2f)
            );

            Rect newRoom = new Rect(pos - size / 2f, size);

            bool overlap = false;
            foreach (Rect r in rooms)
            {
                if (newRoom.Overlaps(r))
                {
                    overlap = true;
                    break;
                }
            }

            if (!overlap)
            {
                rooms.Add(newRoom);
                centers.Add(newRoom.center);
                CreateRoom(newRoom.center, size);
                placed++;
            }
        }

        // === 2. 복도 연결 ===
        ConnectRooms();
    }

    void CreateRoom(Vector2 position, Vector2 size)
    {
        GameObject roomObj = new GameObject("Room");
        roomObj.transform.position = position;

        float halfW = size.x / 2f;
        float halfH = size.y / 2f;
        float t = wallThickness / 2f;

        // 상하좌우 벽만 생성 → 내부는 비어 있음
        CreateWall(roomObj.transform, new Vector2(0, halfH - t), new Vector2(size.x, wallThickness));
        CreateWall(roomObj.transform, new Vector2(0, -halfH + t), new Vector2(size.x, wallThickness));
        CreateWall(roomObj.transform, new Vector2(-halfW + t, 0), new Vector2(wallThickness, size.y));
        CreateWall(roomObj.transform, new Vector2(halfW - t, 0), new Vector2(wallThickness, size.y));

        roomObj.transform.parent = this.transform;
    }

    void CreateWall(Transform parent, Vector2 localPos, Vector2 size)
    {
        GameObject wall = new GameObject("Wall");
        wall.transform.parent = parent;
        wall.transform.localPosition = localPos;
        wall.tag = wallTag;
        wall.layer = LayerMask.NameToLayer(wallLayer);

        SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateWhiteSprite();
        sr.color = wallColor;
        wall.transform.localScale = new Vector3(size.x, size.y, 1f);

        BoxCollider2D col = wall.AddComponent<BoxCollider2D>();
        col.size = Vector2.one;
    }

    void ConnectRooms()
    {
        if (centers.Count < 2) return;

        // X 기준 정렬 후 순서대로 연결
        centers.Sort((a, b) => a.x.CompareTo(b.x));

        for (int i = 0; i < centers.Count - 1; i++)
        {
            Vector2 from = centers[i];
            Vector2 to = centers[i + 1];
            CreateCorridor(from, to);
        }
    }

    void CreateCorridor(Vector2 from, Vector2 to)
    {
        // L자 형태 복도
        Vector2 mid = new Vector2(to.x, from.y);
        CreateCorridorSegment(from, mid);
        CreateCorridorSegment(mid, to);
    }

    void CreateCorridorSegment(Vector2 a, Vector2 b)
    {
        Vector2 dir = b - a;
        float length = dir.magnitude;
        Vector2 center = a + dir / 2f;

        GameObject corridor = new GameObject("Corridor");
        corridor.transform.position = center;

        // 🎨 복도 색상 (시각용)
        SpriteRenderer sr = corridor.AddComponent<SpriteRenderer>();
        sr.sprite = GenerateWhiteSprite();
        sr.color = corridorColor; // 밝은 회색 등

        // ✅ 복도도 Collider 추가 (막힘)
        BoxCollider2D col = corridor.AddComponent<BoxCollider2D>();
        col.size = new Vector2(1f, 1f); // 기본 충돌 크기 (길이/폭은 scale로 결정됨)

        // ✅ 복도 크기와 회전
        corridor.transform.localScale = new Vector3(length, corridorWidth, 1f);
        corridor.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        // ✅ 복도도 벽으로 처리
        corridor.tag = wallTag; // 예: "Unwalkable"
        corridor.layer = LayerMask.NameToLayer(wallLayer); // 예: "Obstacle"

        corridor.transform.parent = this.transform;
    }



    Sprite GenerateWhiteSprite()
    {
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }
}
