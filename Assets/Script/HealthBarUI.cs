using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    private Transform target;
    private Vector3 offset = new Vector3(0, 1.5f, 0);

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void SetFill(float ratio)
    {
        fillImage.fillAmount = ratio;
    }

    void LateUpdate()
    {
        if (target != null)
        {
            Vector3 worldPos = target.position + offset;
            transform.position = Camera.main.WorldToScreenPoint(worldPos);
        }
    }
}
