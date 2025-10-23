using UnityEngine;

public class Weapon : MonoBehaviour
{
    public ItemType weaponType;

    public void Equip(Player player)
    {
        switch (weaponType)
        {
            case ItemType.FireSword:
                player.SetElement(ElementType.Fire);
                break;
            case ItemType.WaterSword:
                player.SetElement(ElementType.Water);
                break;
            case ItemType.WindSword:
                player.SetElement(ElementType.Wind);
                break;
            case ItemType.LightSword:
                player.SetElement(ElementType.LightA);
                break;
            case ItemType.DarkSword:
                player.SetElement(ElementType.Dark);
                break;
            default:
                player.SetElement(ElementType.None);
                break;
        }

        Debug.Log($"무기 장착됨 → {weaponType}, 속성 적용됨: {player.currentElement}");
    }
}
