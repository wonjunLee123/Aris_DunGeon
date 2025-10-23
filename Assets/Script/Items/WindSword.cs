using UnityEngine;

public class WindSword : WeaponBase
{
    private void Awake()
    {
        itemName = "바람의 검";
        description = "🌿 바람의 힘을 지닌 검. 물 속성에게 강하다.";
        Wind = true;
    }
}
