using TMPro;
using UnityEngine;

public class LoadoutRefresher : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI primaryText;
    [SerializeField] TextMeshProUGUI secondaryText;

    public void RefreshPrimary(WeaponData primary)
    {
        Debug.Log("REFRESH PRIMARY CALLED");
        primaryText.text = "Primary: " + (primary != null ? primary.weaponName : "None");
        
    }

    public void RefreshSecondary(WeaponData secondary)
    {
        secondaryText.text = "Secondary: " + (secondary != null ? secondary.weaponName : "None");
    }
}
