using UnityEngine;
using UnityEditor;

public class CreateAllItemData
{
    [MenuItem("Tools/Create Element ItemData (Fire~Dark)")]
    public static void CreateElementItems()
    {
        // 저장 폴더 지정
        string folderPath = "Assets/Script/Items/Data/";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Script/Items", "Data");
        }

        // 아이템 데이터 생성 함수
        void CreateItem(string name, ItemType type, ElementType element)
        {
            ItemData item = ScriptableObject.CreateInstance<ItemData>();
            item.itemName = name;
            item.itemType = type;
            item.elementType = element;
            AssetDatabase.CreateAsset(item, $"{folderPath}{name}.asset");
        }

        // 속성별 아이템 자동 생성
        CreateItem("Fire Sword", ItemType.FireSword, ElementType.Fire);
        CreateItem("Water Sword", ItemType.WaterSword, ElementType.Water);
        CreateItem("Wind Sword", ItemType.WindSword, ElementType.Wind);
        CreateItem("Light Sword", ItemType.LightSword, ElementType.LightA);
        CreateItem("Dark Sword", ItemType.DarkSword, ElementType.Dark);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ 불·물·바람·빛·어둠 검 5종 ItemData 생성 완료!");
    }
}
