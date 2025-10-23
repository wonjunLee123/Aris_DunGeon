using Game.AttackElement;
using UnityEngine;

[System.Serializable]
public class WeaponBase : MonoBehaviour
{
    public string itemName;
    public string description;
    public Sprite icon;

    // 속성
    public bool Fire;
    public bool Water;
    public bool Wind;
    public bool LightA;
    public bool Dark;

    public virtual void Equip(PlayerA player)
    {
        // 기존 속성 초기화
        player.Fire = false;
        player.Water = false;
        player.Wind = false;
        player.LightA = false;
        player.Dark = false;

        // 현재 무기 속성 적용
        player.Fire = Fire;
        player.Water = Water;
        player.Wind = Wind;
        player.LightA = LightA;
        player.Dark = Dark;

        Debug.Log($"⚔️ {itemName} 장착 완료");
    }

    public virtual void Unequip(PlayerA player)
    {
        player.Fire = false;
        player.Water = false;
        player.Wind = false;
        player.LightA = false;
        player.Dark = false;

        Debug.Log($"🛠️ {itemName} 해제됨");
    }
}
