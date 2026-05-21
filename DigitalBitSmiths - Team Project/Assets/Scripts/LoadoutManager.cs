using UnityEngine;

public class LoadoutManager : MonoBehaviour
{
    public static LoadoutManager instance;

    [Header("Weapons")]
    public WeaponData primaryWeapon;
    public WeaponData secondaryWeapon;

    void Awake()
    {
        instance = this;
    }
    public void SetPrimaryWeapon(WeaponData weapon)
    {
        primaryWeapon = weapon;
        Debug.Log("LoadoutManager primary is now: " + weapon.weaponName);
    }

    public void SetSecondaryWeapon(WeaponData weapon)
    {
        secondaryWeapon = weapon;
        Debug.Log("LoadoutManager secondary is now: " + weapon.weaponName);
    }
    public WeaponData GetPrimaryWeapon()
    {
        return primaryWeapon;
    }
    public WeaponData GetSecondaryWeapon()
    {
        return secondaryWeapon;
    }
}