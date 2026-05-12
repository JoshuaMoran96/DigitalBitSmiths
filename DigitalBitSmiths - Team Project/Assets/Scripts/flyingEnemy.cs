using UnityEngine;

public class flyingEnemy : MonoBehaviour, IDamage
{

    [Header("Target")]
    [SerializeField] Transform player;

    [Header("Health")]
    [SerializeField] float maxHealth = 30f;
    [SerializeField] float currentHealth;

    [Header("Detection")]
    [SerializeField] float detectionRange = 10f;

    [Header("Floating")]
    [SerializeField] float floatSpeed = 2f;
    [SerializeField] float floatHeight = 0.25f;

    float startY;

    enemyShoot shootScript;
    Rigidbody2D rb;

    void Start()
    {
        currentHealth = maxHealth;
        startY = transform.position.y;

        rb = GetComponent<Rigidbody2D>();
        shootScript = GetComponent<enemyShoot>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.linearVelocity = Vector2.zero;
        }

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
        float newY = startY + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.x);

        if (shootScript != null && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            shootScript.enabled = distance <= detectionRange;
        }
    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
