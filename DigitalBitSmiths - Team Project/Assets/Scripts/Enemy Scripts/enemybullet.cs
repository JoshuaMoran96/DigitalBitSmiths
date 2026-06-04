using Unity.VisualScripting;
using UnityEngine;

public class enemybullet : MonoBehaviour
{
    public Rigidbody2D rb;

    [Header("Bullet Stats")]
    [SerializeField] public float bulletDamage = 10f;
    [SerializeField] public float bulletSpeed = 15f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.gravityScale = 0;
        rb.linearVelocity = transform.right * bulletSpeed;
        //three second timer 
        Destroy(gameObject, 3f);
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
        }
        Destroy(gameObject);
    }
}
