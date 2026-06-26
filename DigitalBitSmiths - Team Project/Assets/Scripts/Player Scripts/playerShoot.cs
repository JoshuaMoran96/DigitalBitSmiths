using System.Collections.Generic;
using UnityEngine;

public class playerShoot : MonoBehaviour
{
    [Header("Default Weapon Values")]
    public Transform firingPoint;

    [Header("Recoil")]
    [SerializeField] Rigidbody2D rb;

    [Header("Animation & GunSprites")]
    [SerializeField] GameObject gunModel;

    //updating to give each weapon in player arsenal a different cooldown timer instead of sharing the singiular one
    Dictionary<WeaponData, float> weaponNextFireTimes = new Dictionary<WeaponData, float>();
    playerController pm;

    private void Start()
    {
        pm = GetComponent<playerController>();

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

        if (WeaponInventory.instance == null)
        {
            return;
        }

        // Get the currently equipped weapon first
        WeaponData weapon = WeaponInventory.instance.currentWeapon;

        if (weapon == null)
        {
            return;
        }

        // Get this specific weapon's cooldown timer
        weaponNextFireTimes.TryGetValue(weapon, out float nextFireTime);

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot(weapon);
            PlayShootSound(weapon);

            // Set cooldown only for this weapon
            weaponNextFireTimes[weapon] = Time.time + weapon.fireRate;
        }
    }

    void Shoot(WeaponData weapon)
    {
        if (weapon == null || weapon.bulletPrefab == null || firingPoint == null)
        {
            return;
        }

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 shootDirection = (mousePosition - firingPoint.position).normalized;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        int bulletsToFire = weapon.isShotgun ? weapon.pelletCount : 1;

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
                spread = Random.Range(-weapon.spreadAngle, weapon.spreadAngle);
            }

            GameObject newBullet = Instantiate(
                weapon.bulletPrefab,
                firingPoint.position,
                Quaternion.Euler(0f, 0f, angle + spread)
            );

            if (newBullet.TryGetComponent(out bullet bulletComponent))
            {
                float damageToGive = weapon.damage;

                if (pm != null)
                {
                    damageToGive *= pm.GetDamageMultiplier();
                }

                bulletComponent.SetBulletStats(weapon.bulletSpeed, damageToGive);
            }
        }

        ApplyWeaponRecoil(weapon);
    }

    void PlayShootSound(WeaponData weapon)
    {
        if (pm == null || pm.audPlayer == null)
        {
            return;
        }

        if (weapon.shootSound != null && weapon.shootSound.Length > 0)
        {
            pm.audPlayer.PlayOneShot(
                weapon.shootSound[Random.Range(0, weapon.shootSound.Length)],
                weapon.shootSoundVol
            );
        }
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
}