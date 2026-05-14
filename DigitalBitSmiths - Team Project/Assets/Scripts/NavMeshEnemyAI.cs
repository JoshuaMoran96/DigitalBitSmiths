using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using Unity.VisualScripting;

public class NavMeshEnemyAI : MonoBehaviour, IDamage
{
    [Header("Target")]
    [SerializeField] Transform player;

    [Header("Movement")]
    [SerializeField] float detectionRange = 8f;
    [SerializeField] float stopDistance = 1.5f;

    [Header("Health")]
    [SerializeField] float maxHealth = 30f;
    [SerializeField] float currentHealth;

    [Header("Hit Flash")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashTime = 0.1f;

    NavMeshAgent agent;
    Color originalColor;

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

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;
            }
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
        if (player == null || agent == null)
        {
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            gamemanager.instance.updateEnemyCount(-1);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
}
