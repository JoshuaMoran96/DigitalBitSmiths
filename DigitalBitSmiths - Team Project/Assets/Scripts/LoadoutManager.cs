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

    public WeaponData GetPrimaryWeapon()
    {
        return primaryWeapon;
    }
}