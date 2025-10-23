using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject healingPotionPrefab;
    public GameObject weaponPrefab;

    void Start()
    {
        // 🔥 정확한 경로로 로드
        healingPotionPrefab = Resources.Load<GameObject>("HealingPotion");
        weaponPrefab = Resources.Load<GameObject>("IronSword");

        if (healingPotionPrefab == null)
            Debug.LogError("HealingPotion 프리팹을 찾을 수 없습니다!");
        if (weaponPrefab == null)
            Debug.LogError("IronSword 프리팹을 찾을 수 없습니다!");

        SpawnItem(healingPotionPrefab, new Vector2(2, 2));
        SpawnItem(weaponPrefab, new Vector2(4, 4));
    }

    void SpawnItem(GameObject itemPrefab, Vector2 position)
    {
        if (itemPrefab != null)
        {
            Instantiate(itemPrefab, position, Quaternion.identity);
            Debug.Log($"아이템 소환: {itemPrefab.name} at {position}");
        }
        else
        {
            Debug.LogError("아이템 프리팹을 찾을 수 없습니다!");
        }
    }
}
