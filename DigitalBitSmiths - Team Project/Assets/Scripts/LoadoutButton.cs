using UnityEngine;

public class LoadoutButton : MonoBehaviour
{
    [SerializeField] WeaponData weapon;
    [SerializeField] bool isPrimary;

    public void SelectWeapon()
    {
        Debug.Log("Button clicked: " + gameObject.name);

        if (LoadoutManager.instance == null)
        {
            Debug.LogWarning("No LoadoutManager instance found.");
            return;
        }

        if (weapon == null)
        {
            Debug.LogWarning("No weapon assigned on " + gameObject.name);
            return;
        }

        if (isPrimary)
        {
            LoadoutManager.instance.SetPrimaryWeapon(weapon);
            Debug.Log("Set primary to: " + weapon.weaponName);
        }
        else
        {
            LoadoutManager.instance.SetSecondaryWeapon(weapon);
            Debug.Log("Set secondary to: " + weapon.weaponName);
        }
    }
}