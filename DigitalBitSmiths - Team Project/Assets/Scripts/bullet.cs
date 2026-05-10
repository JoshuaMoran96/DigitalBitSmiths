using UnityEngine;

public class bullet : MonoBehaviour
{
   //Bullet stats
    public float bulletSpeed = 15f;
    public float bulletDamage = 10f;
    public Rigidbody2D rb;

    private void FixedUpdate()
    {
        rb.linearVelocity = Vector2.right * bulletSpeed;
    }

    //destroy bullet as soon as it makes any contact
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
