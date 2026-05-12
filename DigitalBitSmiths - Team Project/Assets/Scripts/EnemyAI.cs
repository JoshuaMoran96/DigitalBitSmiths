using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashTime = 0.1f;

    Color originalColor;
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

    private float currentHealth;
    private Rigidbody2D rb;

    private void Start()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        originalColor = spriteRenderer.color;
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
    public void takeDamage(float amount)
    {
        currentHealth -= amount;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateGameGoal(-1);
            }

            Destroy(gameObject);
        }
    }

    //damage player on touch
    private void OnCollisionEnter2D(Collision2D collision)
    {
        IDamage dmg = collision.gameObject.GetComponent<IDamage>();

        if (dmg != null && collision.gameObject.CompareTag("Player"))
        {
            dmg.takeDamage(touchDamage);
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

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        spriteRenderer.color = originalColor;
    }
}