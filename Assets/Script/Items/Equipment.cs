using UnityEngine;

public class Equipment : MonoBehaviour
{
    public ItemData equippedItem;
    private PlayerA player;

    void Start()
    {
        player = FindFirstObjectByType<PlayerA>();
    }

    public void Equip(ItemData newItem)
    {
        if (newItem == null || player == null)
            return;

        equippedItem = newItem;
        player.ApplyElement(equippedItem.elementType);  // ✅ 여기 수정
        Debug.Log($"⚔️ {equippedItem.itemName} 장착됨 ({equippedItem.elementType})");
    }
}
