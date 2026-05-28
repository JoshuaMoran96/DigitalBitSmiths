using UnityEngine;
using UnityEngine.UI;

public class BossShieldBar : MonoBehaviour
{
    [SerializeField] Image fill;

    public void UpdateShieldBar(int currentHits, int maxHits)
    {
        if (fill == null)
        {
            return;
        }

        float value =
            1f - ((float)currentHits / maxHits);

        fill.fillAmount = Mathf.Clamp01(value);
    }
}