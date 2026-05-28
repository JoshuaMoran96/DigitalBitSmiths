using UnityEngine;

public class bullet : MonoBehaviour
{
    public float bulletSpeed = 15f;
    public float bulletDamage = 10f;
    public float bulletDestroyTime = 3.0f;
    public Rigidbody2D rb;

    bool statsSet;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (!statsSet)
        {
            Launch();
        }


        Destroy(gameObject, bulletDestroyTime);
    }

    public void SetBulletStats(float speed, float damage)
    {
        bulletSpeed = speed;
        bulletDamage = damage;
        statsSet = true;

        Launch();
    }

    void Launch()
    {
        if (rb != null)
        {
            rb.linearVelocity = transform.right * bulletSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }

        BossMissile missile = collision.gameObject.GetComponent<BossMissile>();

        if (missile != null)
        {
            if (missile != null)
            {
                if (missile.CanReflect())
                {
                    missile.Reflect();
                    Destroy(gameObject);
                }

                return;
            }
        }

        IDamage dmg = collision.gameObject.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(bulletDamage);
        }

        Destroy(gameObject);
    }
}