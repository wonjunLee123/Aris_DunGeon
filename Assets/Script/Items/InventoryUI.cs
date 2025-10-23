using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("UI 연결")]
    public GameObject inventoryPanel;
    public GameObject itemButtonPrefab;
    public Transform contentParent;

    private InventorySystem inventory;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // ✅ InventorySystem 존재 확인
        if (InventorySystem.Instance == null)
        {
            Debug.LogWarning("⚠️ InventorySystem.Instance가 존재하지 않습니다! 씬에 InventorySystem 오브젝트가 있는지 확인하세요.");
            return;
        }

        inventory = InventorySystem.Instance;

        if (inventory.items == null)
            inventory.items = new List<ItemData>();

        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (inventoryPanel != null)
                inventoryPanel.SetActive(!inventoryPanel.activeSelf);

            RefreshInventory();
        }
    }

    public void RefreshInventory()
    {
        // ✅ UI 연결 확인
        if (inventoryPanel == null || itemButtonPrefab == null || contentParent == null)
        {
            Debug.LogError("❌ 인벤토리 UI 연결이 누락됨! (Panel, Prefab, Parent 중 하나가 null)");
            return;
        }

        // ✅ 인벤토리 시스템 존재 여부 확인
        if (inventory == null)
        {
            inventory = InventorySystem.Instance;
            if (inventory == null)
            {
                Debug.LogError("❌ InventorySystem.Instance를 찾을 수 없습니다!");
                return;
            }
        }

        // ✅ 리스트 null 보호
        if (inventory.items == null)
        {
            Debug.LogWarning("⚠️ InventorySystem.items가 null이라 새로 생성합니다.");
            inventory.items = new List<ItemData>();
        }

        // 기존 슬롯 삭제
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        // 아이템 버튼 생성
        foreach (var item in inventory.items)
        {
            if (item == null)
            {
                Debug.LogWarning("⚠️ null 아이템이 인벤토리에 포함되어 있습니다. 무시합니다.");
                continue;
            }

            GameObject buttonObj = Instantiate(itemButtonPrefab, contentParent);

            // ✅ Text 연결 체크
            Text txt = buttonObj.GetComponentInChildren<Text>();
            if (txt != null)
                txt.text = item.itemName;
            else
                Debug.LogWarning("⚠️ itemButtonPrefab에 Text 컴포넌트가 없습니다!");

            // ✅ 버튼 클릭 이벤트 연결
            Button btn = buttonObj.GetComponent<Button>();
            if (btn != null)
                btn.onClick.AddListener(() => inventory.EquipItem(item));
        }
    }
}
