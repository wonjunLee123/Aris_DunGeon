// PlayerA.cs
using UnityEngine;

public class PlayerA : MonoBehaviour
{
    public bool Fire;
    public bool Water;
    public bool Wind;
    public bool LightA;
    public bool Dark;

    public void ResetAllElements()
    {
        Fire = Water = Wind = LightA = Dark = false;
    }

    public void ApplyElement(ElementType element)
    {
        ResetAllElements();
        switch (element)
        {
            case ElementType.Fire:
                Fire = true; Debug.Log("🔥 불 속성 활성화!"); break;
            case ElementType.Water:
                Water = true; Debug.Log("💧 물 속성 활성화!"); break;
            case ElementType.Wind:
                Wind = true; Debug.Log("🌪 바람 속성 활성화!"); break;
            case ElementType.LightA:
                LightA = true; Debug.Log("💡 빛 속성 활성화!"); break;
            case ElementType.Dark:
                Dark = true; Debug.Log("🌑 어둠 속성 활성화!"); break;
        }
    }
}
