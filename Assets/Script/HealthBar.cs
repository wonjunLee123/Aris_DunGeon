using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;

    public void SetMaxHealth(int maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetHealth(int currentHealth)
    {
        healthSlider.value = currentHealth;
    }

    // ü�¹� ��ġ�� �÷��̾� �Ӹ� ���� ����
    public void UpdatePosition(Vector3 worldPosition)
    {
        transform.position = worldPosition + new Vector3(0, 1.5f, 0);
    }
}
