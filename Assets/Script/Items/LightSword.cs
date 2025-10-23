using UnityEngine;

public class LightSword : WeaponBase
{
    private void Awake()
    {
        itemName = "빛의 검";
        description = "☀️ 빛의 힘을 지닌 검. 어둠 속성에게 강하다.";
        LightA = true;
    }
}
