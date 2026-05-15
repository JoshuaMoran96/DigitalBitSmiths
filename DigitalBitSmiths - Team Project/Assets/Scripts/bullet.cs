using UnityEngine;

public class bullet : MonoBehaviour
{
   //Bullet stats
    public float bulletSpeed = 15f;
    public float bulletDamage = 10f;
    public Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //move bullet forward
        rb.linearVelocity = transform.right * bulletSpeed;
        //destroy bullet after 3 seconds
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //check if bullet hit a boss missile
        BossMissile missile = 
            collision.gameObject.GetComponent<BossMissile>();

        //reflect missile instead of damaging it
        if (missile != null)
        {
            missile.Reflect();
            Destroy(gameObject);
            return;
        }

        //normal damage logic
        IDamage dmg = collision.gameObject.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(bulletDamage);
        }
        Destroy(gameObject);
    }
}
