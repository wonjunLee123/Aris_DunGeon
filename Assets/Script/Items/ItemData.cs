using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    public ElementType elementType; // �Ӽ� ���� (Fire, Water, Wind, LightA, Dark)
}
