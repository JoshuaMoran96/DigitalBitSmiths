using System.Collections.Generic;
using UnityEngine;

public class WeaponInventory : MonoBehaviour
{
    public static WeaponInventory instance;

    [Header("Starting Weapon")]
    [SerializeField] WeaponData startingWeapon;

    [Header("Owned Weapons")]
    [SerializeField] List<WeaponData> ownedWeapons = new List<WeaponData>();

    public WeaponData currentWeapon;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (startingWeapon != null)
        {
            AddWeapon(startingWeapon);
            EquipWeapon(startingWeapon);
        }
    }

    public void AddWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            return;
        }

        if (!ownedWeapons.Contains(weapon))
        {
            ownedWeapons.Add(weapon);
        }
    }

    public void EquipWeapon(WeaponData weapon)
    {
        if (weapon == null)
        {
            return;
        }
        if (!ownedWeapons.Contains(weapon))
        {
            return;
        }

        currentWeapon = weapon;
        Debug.Log("Equippied: " + weapon.weaponName);

        // update the UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateWeaponDisplay();

        }

    }

    public List<WeaponData> GetOwnedWeapons()
    {
        return ownedWeapons;
    }

    public bool HasWeapon(WeaponData weapon)
    {
        return ownedWeapons.Contains(weapon);
    }
}