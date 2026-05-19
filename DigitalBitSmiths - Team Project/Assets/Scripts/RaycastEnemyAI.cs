using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class RaycastEnemyAI : MonoBehaviour, IDamage
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float patrolSpeed;
    [SerializeField] float chaseSpeed;
    [SerializeField] bool movingRight = true;
    [SerializeField] float flipCooldown = 0.25f;


    [Header("Player Detection")]
    [SerializeField] bool playerInRange;
    [SerializeField] float attackDistance = 1.5f;
    [SerializeField] float verticalAttackRange = 1.2f;
    [SerializeField] float touchDamage = 5f;
    [SerializeField] float damageRate = 0.5f;

    [Header("Raycast")]
    [SerializeField] Transform wallCheck;
    [SerializeField] Transform ledgeCheck;
    [SerializeField] float wallCheckDistance = 0.4f;
    [SerializeField] float ledgeCheckDistance = 1f;
    [SerializeField] LayerMask groundLayer;

    [Header("Health")]
    [SerializeField] float maxHealth = 30f;
    [SerializeField] float currentHealth;

    [Header("Hit Flash")]
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashTime = 0.1f;

    bool playerInAttackRange;
    float nextDamageTime;
    Vector3 wallCheckStartPos;
    Vector3 ledgeCheckStartPos;
    float nextFlipTime;
    Rigidbody2D rb;
    Color originalColor;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        wallCheckStartPos = wallCheck.localPosition;
        ledgeCheckStartPos = ledgeCheck.localPosition;
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        UpdateCheckPoints();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        if (playerInRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
            CheckForTurnAround();
        }
    }

    void Patrol()
    {
        float speed = patrolSpeed;

        if (movingRight)
        {
            rb.linearVelocity = new Vector2(speed, rb.linearVelocity.y);
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, rb.linearVelocity.y);
        }
    }

    void CheckForTurnAround()
    {
        Vector2 wallDirection;

        if (movingRight)
        {
            wallDirection = Vector2.right;
        }
        else
        {
            wallDirection = Vector2.left;
        }

        RaycastHit2D wallHit = Physics2D.Raycast(
            wallCheck.position,
            wallDirection,
            wallCheckDistance,
            groundLayer
            );

        RaycastHit2D groundHit = Physics2D.Raycast(
            ledgeCheck.position,
            Vector2.down,
            ledgeCheckDistance,
            groundLayer
            );

        if (wallHit.collider != null || groundHit.collider == null)
        {
            Flip();
        }
    }

    void Flip()
    {
        if (Time.time < nextFlipTime)
        {
            return;
        }
        nextFlipTime = Time.time + flipCooldown;

        movingRight = !movingRight;
        if (spriteRenderer != null)
        {
            flipChar(!movingRight);
        }
        UpdateCheckPoints();
    }

    public void takeDamage(float amount)
    {
        currentHealth -= amount;
        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            if(gamemanager.instance != null)
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

    private void OnDrawGizmos()
    {
        if (wallCheck != null)
        {
            Gizmos.color = Color.red;

            Vector3 dir = movingRight ? Vector3.right : Vector3.left;
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + dir * wallCheckDistance);
        }

        if (ledgeCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(ledgeCheck.position, ledgeCheck.position + Vector3.down * ledgeCheckDistance);
        }
    }

    void UpdateCheckPoints()
    {
        float direction = movingRight ? 1f : -1f;

        if (wallCheck != null)
        {
            Vector3 pos = wallCheckStartPos;
            pos.x = Mathf.Abs(wallCheckStartPos.x) * direction;
            wallCheck.localPosition = pos;
        }

        if (ledgeCheck != null)
        {
            Vector3 pos = ledgeCheckStartPos;
            pos.x = Mathf.Abs(ledgeCheckStartPos.x) * direction;
            ledgeCheck.localPosition = pos;
        }
    }

    void ChasePlayer()
    {
        if (playerInAttackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (Time.time >= nextDamageTime)
            {
                IDamage dmg = gamemanager.instance.player.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(touchDamage);
                }

                nextDamageTime = Time.time + damageRate;
            }

            return;
        }

        if (gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return;
        }

        Transform player = gamemanager.instance.player.transform;

        float xDistance = Mathf.Abs(transform.position.x - player.position.x);
        float yDistance = Mathf.Abs(transform.position.y - player.position.y);

        if (xDistance <= attackDistance && yDistance <= verticalAttackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

            if (Time.time >= nextDamageTime)
            {
                IDamage dmg = gamemanager.instance.player.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(touchDamage);
                }

                nextDamageTime = Time.time + damageRate;
            }

            return;
        }

        if (player.position.x > transform.position.x)
        {
            movingRight = true;
            UpdateCheckPoints();

            if (!HasGroundAhead())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }

            rb.linearVelocity = new Vector2(chaseSpeed, rb.linearVelocity.y);
            flipChar(false);
        }
        else
        {
            movingRight = false;
            UpdateCheckPoints();

            if (!HasGroundAhead())
            {
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                return;
            }

            rb.linearVelocity = new Vector2(-chaseSpeed, rb.linearVelocity.y);
            flipChar(true);
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

    void flipChar(bool flip)
    {


        spriteRenderer.flipX = flip;


    }

    bool HasGroundAhead()
    {
        RaycastHit2D groundHit = Physics2D.Raycast(
            ledgeCheck.position,
            Vector2.down,
            ledgeCheckDistance,
            groundLayer
        );

        return groundHit.collider != null;
    }

    public void SetPlayerInAttackRange(bool value)
    {
        playerInAttackRange = value;
    }

    public void ResetPlayerDetection()
    {
        playerInRange = false;
        playerInAttackRange = false;
    }
}
