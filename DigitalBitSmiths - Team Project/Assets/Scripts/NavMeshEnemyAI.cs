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
    //tracks if player is inside detection trigger
    bool playerInRange;
    //timer for repeated damage
    float nextDamageTime;

    void Start()
    {
        //get navmesh agent component
        agent = GetComponent<NavMeshAgent>();

        currentHealth = maxHealth;

        //auto assign sprite renderer if missing
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        //save original sprite color
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // configure nav mesh settings for 2D
        if (agent != null)
        {
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.stoppingDistance = stopDistance;
        }
    }

    void Update()
    {
        // stop checking player distance if player is outside trigger
        if (!playerInRange)
        {
            if (agent != null && agent.isOnNavMesh)
            {
                agent.ResetPath();
            }

            return;
        }

        //safety checks
        if (gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            return;
        }

        // get player from gamemanager
        Transform player = gamemanager.instance.player.transform;

        // move toward player
        agent.SetDestination(player.position);


        //check distance to player
        float distance = Vector2.Distance(transform.position, player.position);


        // damage player when close enough
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


    //player entered detection trigger
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    //player left detection trigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    //called when enemy takes damage
    public void takeDamage(float amount)
    {
        currentHealth -= amount;

        StartCoroutine(FlashRed());

        //destroy enemy if health reaches 0
        if (currentHealth <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateEnemyCount(-1);
            }

            Destroy(gameObject);
        }
    }

    //flash enemy red when hit
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