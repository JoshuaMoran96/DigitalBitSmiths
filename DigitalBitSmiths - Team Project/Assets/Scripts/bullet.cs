using UnityEngine;

public class bullet : MonoBehaviour
{
   //Bullet stats
    public float bulletSpeed = 15f;
    public float bulletDamage = 10f;
    public Rigidbody2D rb;

    void Start()
    {
        rb.linearVelocity = transform.right * bulletSpeed;
        //three second timer 
        Destroy(gameObject, 3f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy(gameObject);
    }
}
