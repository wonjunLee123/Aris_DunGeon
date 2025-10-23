using UnityEngine;

public class Player : MonoBehaviour
{
    public ElementType currentElement = ElementType.None;

    // 기존 ApplyElement 그대로 둬도 OK
    public void ApplyElement(ElementType element)
    {
        currentElement = element;
        Debug.Log($"속성 변경됨 → {element}");
    }

    // ✅ 다른 스크립트에서 호출할 수 있게 SetElement로 별칭 추가
    public void SetElement(ElementType element)
    {
        ApplyElement(element);
    }
}
