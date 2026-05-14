using UnityEngine;
using System.Collections;

public class flyingEnemy : MonoBehaviour, IDamage
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashTime = 0.1f;

    Color originalColor;
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
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        originalColor = spriteRenderer.color;
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
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (shootScript != null && player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            shootScript.enabled = distance <= detectionRange;
        }
    }

    public void takeDamage(float amount)
    {

        StartCoroutine(FlashRed());
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            gamemanager.instance.updateEnemyCount(-1);
            Destroy(gameObject);

        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        spriteRenderer.color = originalColor;
    }
}
