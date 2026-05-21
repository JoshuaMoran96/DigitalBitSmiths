using Unity.VisualScripting;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float lifeTime = 3f;
    [SerializeField] float launchForce = 12f;

    [Header("Explosion")]
    [SerializeField] float explosionRadius = 2f;
    [SerializeField] float explosionDamage = 25f;
    [SerializeField] LayerMask damageLayers;
    [SerializeField] GameObject explostionEffect;

    [SerializeField] float spriteRotationOffset = 0f;
    Rigidbody2D rb;
    Collider2D col;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (col != null)
        {
            col.enabled = false;
            Invoke(nameof(EnableCollider), 0.1f);
        }
        Debug.Log("Grenade Launched");
        if (rb != null)
        {
            rb.linearVelocity = transform.right * launchForce;
        }

        Invoke(nameof(Explode), lifeTime);
    }

    void Update()
    {
        if (rb != null && rb.linearVelocity.sqrMagnitude > 0.01f){
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(
                0f,
                0f,
                angle + spriteRotationOffset
            );
        }
    }

    void EnableCollider()
    {
        if (col != null)
        {
            col.enabled = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Grende hit: " + collision.gameObject.name);
        Explode();
    }
    
    void Explode()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius,
            damageLayers
            );

        for (int i =0; i < hits.Length; i++)
        {
            IDamage dmg = hits[i].GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(explosionDamage);
            }
        }

        if (explostionEffect != null)
        {
            Instantiate(explostionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

}
