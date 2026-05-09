using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float stopDistance = 1f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Damage")]
    [SerializeField] private int touchDamage = 1;

    private int currentHealth;
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

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

    private void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > stopDistance)
        {
            float directionX = player.position.x - transform.position.x;

            if (directionX > 0)
            {
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            }
            else if (directionX < 0)
            {
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    //damage function
    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    //damage player on touch
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player took damage: " + touchDamage);

            //if/when we make a health script it could go here
        }
    }

    //Visualize detection range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}