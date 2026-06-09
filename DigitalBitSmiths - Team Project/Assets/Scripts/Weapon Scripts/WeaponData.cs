using UnityEngine;

[CreateAssetMenu(menuName = "Loadout/Weapon")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public GameObject bulletPrefab;

    public float fireRate = 0.2f;
    public float bulletSpeed = 15f;
    public float damage = 10f;

    [Header("Shotgun")]
    public bool isShotgun;
    public int pelletCount = 5;

    [Header("Recoil")]
    public float recoilForce = 0f;

    [Header("Accuracy")]
    public bool useSpread;
    public float spreadAngle = 0f;

    [Header("----- Audio -----")]
    public AudioClip[] shootSound;
    [Range(0, 1)] public float shootSoundVol;

    public Sprite weaponIcon;
}
