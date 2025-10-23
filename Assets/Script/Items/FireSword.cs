using UnityEngine;

public class FireSword : WeaponBase
{
    private void Awake()
    {
        itemName = "불의 검";
        description = "🔥 불의 힘을 지닌 검. 바람 속성에게 강하다.";
        Fire = true;
    }
}
