using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class NavMeshEnemyAI : MonoBehaviour, IDamage
{
    [Header("Movement")]
    [SerializeField] float stopDistance = 1.5f;

    [Header("Health")]
    [SerializeField] float maxHealth = 30f;
    [SerializeField] float currentHealth;

    [Header("Damage")]
    [SerializeField] float touchDamage = 5f;
    [SerializeField] float damageRate = 0.5f;

    [Header("Hit Flash")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashTime = 0.1f;

    NavMeshAgent agent;
    Color originalColor;
    bool playerInRange;
    float nextDamageTime;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        if (!playerInRange)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }

            return;
        }

        if (gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        Transform player = gamemanager.instance.player.transform;

        agent.SetDestination(player.position);

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= stopDistance)
        {
            IDamage dmg = gamemanager.instance.player.GetComponent<IDamage>();

            if (dmg != null && Time.time >= nextDamageTime)
            {
                dmg.takeDamage(touchDamage);
                nextDamageTime = Time.time + damageRate;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateEnemyCount(-1);
            }

            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        spriteRenderer.color = originalColor;
    }
}