using UnityEngine;

public class enemyShoot : MonoBehaviour
{
    
    public GameObject enemyBulletPrefab;
    public Transform firePoint;

    [Header("Attack")]
    public float fireRate = 1f;
    public float detectionRange = 10f;



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
        {
            return;
        }

        float distance =
            Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            detectionRange
            );
    }
}

//logic path for enemy gunner to fire at the player using its specific bullet
