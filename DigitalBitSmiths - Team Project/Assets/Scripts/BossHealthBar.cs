using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour
{
    [SerializeField] Image fill;

    public void UpdateHealthBar(float currentHealth, float maxHealth)
    {
        if (fill == null)
        {
            return;
        }

        fill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
    }
}