using UnityEngine;

public class DarkSword : WeaponBase
{
    private void Awake()
    {
        itemName = "어둠의 검";
        description = "🌑 어둠의 힘을 지닌 검. 빛 속성에게 강하다.";
        Dark = true;
    }
}
