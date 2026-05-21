using UnityEngine;

[CreateAssetMenu(menuName = "Loadout/Weapon")]
public class WeaponData : ScriptableObject
{
    [Header("Shotgun")]
    public bool isShotgun;
    public int pelletCount = 5;
    public float spreadAngle = 20f;
    public string weaponName;

    [Header("Recoil")]
    public float recoilForce = 0f;

    public GameObject bulletPrefab;

    public float damage = 10f;
    public float fireRate = 0.2f;
    public float bulletSpeed = 15f;

    public Sprite weaponIcon;
}
