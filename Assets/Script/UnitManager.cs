using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    private Dictionary<Vector2Int, GameObject> unitMap = new Dictionary<Vector2Int, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // ✅ 매니저가 씬 전환돼도 유지
        }
        else if (Instance != this)
        {
            Debug.LogWarning($"중복된 {nameof(UnitManager)} 발견 → {gameObject.name} 제거됨");
            Destroy(gameObject);
            return;
        }
    }


    public Vector2Int WorldToTilePos(Vector2 worldPos)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPos.x),
            Mathf.RoundToInt(worldPos.y)
        );
    }


    public void RegisterUnit(Vector2Int tilePos, GameObject unit)
    {
        if (!unitMap.ContainsKey(tilePos))
            unitMap.Add(tilePos, unit);
    }

    public void MoveUnit(Vector2Int from, Vector2Int to, GameObject unit)
    {
        if (unitMap.ContainsKey(from))
            unitMap.Remove(from);

        if (!unitMap.ContainsKey(to))
            unitMap.Add(to, unit);
    }

    public void UnregisterUnit(GameObject unit)
    {
        Vector2Int tile = WorldToTilePos(unit.transform.position);
        if (unitMap.ContainsKey(tile) && unitMap[tile] == unit)
            unitMap.Remove(tile);
    }

    public GameObject GetUnitAtPosition(Vector2Int pos)
    {
        if (unitMap.ContainsKey(pos))
            return unitMap[pos];
        return null;
    }
}
