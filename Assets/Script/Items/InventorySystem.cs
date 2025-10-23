using System.Collections.Generic;
using UnityEngine;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    public List<ItemData> items = new List<ItemData>();
    private PlayerA player;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerA>();
    }

    public void AddItem(ItemData newItem)
    {
        if (newItem == null) return;

        items.Add(newItem);
        Debug.Log($"🎒 아이템 추가: {newItem.itemName}");
        InventoryUI.Instance.RefreshInventory();
    }

    public void EquipItem(ItemData item)
    {
        if (item == null || player == null) return;

        player.ApplyElement(item.elementType); // ✅ 이름 통일
        Debug.Log($"⚡ 속성 변경: {item.itemName} → {item.elementType}");
    }
}
