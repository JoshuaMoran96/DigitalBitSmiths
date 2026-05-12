using UnityEngine;

public class enemyShoot : MonoBehaviour
{
    public GameObject enemyBulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    float nextFireTime;

    public Transform player;

    public void Awake()
    {
        //automatically find player if not assigned
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }
    void Update()
    {
        if (player == null)
            return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        Vector2 direction = player.position - firePoint.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        Instantiate(
            enemyBulletPrefab,
            firePoint.position,
            Quaternion.Euler(0f, 0f, angle)
        );
    }
}

//logic path for enemy gunner to fire at the player using its specific bullet
