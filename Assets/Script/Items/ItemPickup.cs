using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData data; // 아이템 데이터 (인스펙터에서 연결)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 플레이어에 닿았는지 확인
        PlayerMovement player = collision.GetComponent<PlayerMovement>();
        if (player == null) return;

        if (data == null)
        {
            Debug.LogWarning($"⚠️ {gameObject.name} 의 ItemData가 비어 있습니다!");
            return;
        }

        // 인벤토리에 아이템 추가
        if (InventorySystem.Instance != null)
        {
            InventorySystem.Instance.AddItem(data);
            Debug.Log($"✅ {data.itemName} 을(를) 인벤토리에 추가했습니다.");
        }
        else
        {
            Debug.LogError("❌ InventorySystem.Instance 를 찾을 수 없습니다!");
        }

        // 아이템 비활성화 (씬에서 사라지게)
        gameObject.SetActive(false);
    }
}
