using UnityEngine;

public class WaterSword : WeaponBase
{
    private void Awake()
    {
        itemName = "물의 검";
        description = "💧 물의 힘을 지닌 검. 불 속성에게 강하다.";
        Water = true;
    }
}
