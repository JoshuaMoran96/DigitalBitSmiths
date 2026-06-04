using UnityEngine;

public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] Transform fill;

    public void UpdateHealthBar(float currenthealth, float maxHealth)
    {
        if (fill == null)
        {
            return;
        }

        float healthPercent = Mathf.Clamp01(currenthealth / maxHealth);

        fill.localScale = new Vector3(
            healthPercent,
            fill.localScale.y,
            fill.localScale.z
        );

        fill.localPosition = new Vector3(
            -(1f - healthPercent) * 0.5f,
            fill.localPosition.y,
            fill.localPosition.z
        );
    }
}
