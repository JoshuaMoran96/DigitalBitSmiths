using UnityEngine;

public class LMGEnemyBullet : MonoBehaviour
{
    [Range(1, 100)][SerializeField] float bulletSpeed = 18f;
    [Range(1, 100)][SerializeField] float bulletDamage = 4f;
    [Range(1, 10)][SerializeField] float lifeTime = 3f;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = transform.right * bulletSpeed;
        }

        Destroy(gameObject, lifeTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IDamage dmg = collision.gameObject.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(bulletDamage);
            }

            Destroy(gameObject);
            return;
        }

        if (!collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}