using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class playerShoot : MonoBehaviour
{
    [Header("Default Weapon Values")]
    public float fireRate = 0.2f;
    public Transform firingPoint;
    public GameObject bulletPrefab;
    //public Rigidbody2D bulletRB;

    [Header("Alt-Fire")]
    [SerializeField] int altFireCount;

    [Header("Loadout")]
    [SerializeField] WeaponData currentWeapon;
    [SerializeField] WeaponData secondaryWeapon;

    [Header("Recoil")]
    [SerializeField] Rigidbody2D rb;

    gamemanager instance;
    float primaryNextFireTime;
    float secondaryNextFireTime;

    //attempting to update for damage boost
    
    playerController pm;

    private void Start()
    {
        pm = GetComponent<playerController>();

        if (LoadoutManager.instance != null)
        {
            currentWeapon = LoadoutManager.instance.primaryWeapon;
            secondaryWeapon = LoadoutManager.instance.secondaryWeapon;
        }

        if (currentWeapon == null && LoadoutManager.instance != null)
        {
            currentWeapon = LoadoutManager.instance.GetPrimaryWeapon();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
   }

    private void Update()
    {
        if (gamemanager.instance != null && gamemanager.instance.isPaused)
        {
            return;
        }

        if (Input.GetMouseButton(0) && currentWeapon != null && Time.time >= primaryNextFireTime)
        {
            Shoot(currentWeapon);
            if (currentWeapon.shootSound.Length != 0)
                pm.audPlayer.PlayOneShot(currentWeapon.shootSound[Random.Range(0, currentWeapon.shootSound.Length)], currentWeapon.shootSoundVol);
            primaryNextFireTime = Time.time + currentWeapon.fireRate;
        }

        if (Input.GetMouseButton(1) && secondaryWeapon != null && Time.time >= secondaryNextFireTime)
        {
            Shoot(secondaryWeapon);
            if(secondaryWeapon.shootSound.Length != 0)
            pm.audPlayer.PlayOneShot(secondaryWeapon.shootSound[Random.Range(0, secondaryWeapon.shootSound.Length)], secondaryWeapon.shootSoundVol);

            secondaryNextFireTime = Time.time + secondaryWeapon.fireRate;
        }
    }

    //updating to have players firing be directed and aimed by mouse 
    void Shoot(WeaponData weapon)
    {
        if (weapon == null)
        {
            return;
        }

        GameObject activeBulletPrefab = weapon.bulletPrefab;

        if (activeBulletPrefab == null || firingPoint == null)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 shootDirection = (mousePosition - firingPoint.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        int bulletsToFire = 1;

        if (weapon.isShotgun)
        {
            bulletsToFire = weapon.pelletCount;
        }
        for (int i = 0; i < bulletsToFire; i++)
        {
            float spread = 0f;

            if (weapon.isShotgun && bulletsToFire > 1)
            {
                float startAngle = -weapon.spreadAngle;
                float angleStep = (weapon.spreadAngle * 2f) / (bulletsToFire - 1);
                spread = startAngle + angleStep * i;
            }
            else if (weapon.useSpread)
            {
                spread = Random.Range(
                    -weapon.spreadAngle,
                    weapon.spreadAngle
                    );
            }

            GameObject newBullet = Instantiate(
                activeBulletPrefab,
                firingPoint.position,
                Quaternion.Euler(0f, 0f, angle + spread)
            );

            if (newBullet.TryGetComponent(out bullet bulletComponent))
            {
                //updating to retrieve new weapon damage values
                float damageToGive = weapon.damage;
                if(pm != null)
                {
                    damageToGive *= pm.GetDamageMultiplier();
                }

                bulletComponent.SetBulletStats(weapon.bulletSpeed, damageToGive);
            }

        }
        ApplyWeaponRecoil(weapon);


    }

    void ApplyRecoil()
    {
        if (rb == null || firingPoint == null)
        {
            return;
        }

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - (Vector2)firingPoint.position).normalized;

        Vector2 recoil = -direction * 20f;


        rb.linearVelocity += recoil;
    }

    void ApplyWeaponRecoil(WeaponData weapon)
    {
        if (weapon == null || rb == null || firingPoint == null)
        {
            return;
        }

        if (weapon.recoilForce <= 0f)
        {
            return;
        }

        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 shootDirection = (mouseWorld - (Vector2)firingPoint.position).normalized;

        Vector2 recoil = -shootDirection * weapon.recoilForce;

        rb.linearVelocity += recoil;
    }

    public void RefreshWeapons()
    {
        if (LoadoutManager.instance == null)
        {
            return;
        }

        if (LoadoutManager.instance.primaryWeapon != null)
        {
            currentWeapon = LoadoutManager.instance.primaryWeapon;
            Debug.Log("Player primary: " + currentWeapon.weaponName);
        }
        if (LoadoutManager.instance.secondaryWeapon != null)
        {
            secondaryWeapon = LoadoutManager.instance.secondaryWeapon;
            Debug.Log("Player secondary: " + secondaryWeapon.weaponName);
        }
    }
}
