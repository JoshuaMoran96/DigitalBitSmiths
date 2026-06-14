using UnityEngine;

public class SuperiorJoe : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    [Header("----- Target -----")]
    public Transform target;

    [Header("----- Movement -----")]
    public float moveSpeed = 6f;
    public float chaseDistance = 12f;
    public float stopDistance = 1.5f;

    [Header("----- Jumping -----")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundMask;
    public float jumpForce = 12f;
    public float obstacleCheckDistance = 0.75f;

    private bool isGrounded;

    [Header("----- Dash -----")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1.5f;
    private float nextDashTime = 0f;
    private bool isDashing;

    [Header("----- Dash Reaction -----")]
    public float dashTriggerRadius = 3f;

    [Header("----- Health -----")]
    public float maxHP = 150f;
    private float currentHP;
    private Color origColor;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        origColor = spriteRenderer.color;
        currentHP = maxHP;
    }

    void Update()
    {
        GroundCheck();

        if (!isDashing)
        {
            HandleMovement();
            HandleJumping();
        }
    }

    // ---------------------------
    // GROUND CHECK
    // ---------------------------
    void GroundCheck()
    {
        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundMask
        );
    }

    // ---------------------------
    // MOVEMENT
    // ---------------------------
    void HandleMovement()
    {
        float distance = Mathf.Abs(target.position.x - transform.position.x);

        if (distance > chaseDistance)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        if (distance < stopDistance)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        float direction = Mathf.Sign(target.position.x - transform.position.x);

        // Flip sprite
        spriteRenderer.flipX = direction < 0;

        // Move
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    // ---------------------------
    // JUMPING
    // ---------------------------
    void HandleJumping()
    {
        float dir = spriteRenderer.flipX ? -1 : 1;

        Vector2 feet = (Vector2)transform.position + new Vector2(0, -0.2f);
        Vector2 chest = (Vector2)transform.position + new Vector2(0, 0.5f);

        bool wallLow = Physics2D.Raycast(feet, Vector2.right * dir, obstacleCheckDistance, groundMask);
        bool wallHigh = Physics2D.Raycast(chest, Vector2.right * dir, obstacleCheckDistance, groundMask);

        bool gapAhead = !Physics2D.Raycast(feet + new Vector2(dir * 0.5f, 0), Vector2.down, 1f, groundMask);

        if ((wallLow || wallHigh || gapAhead) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    // ---------------------------
    // DASH REACTION
    // ---------------------------
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bullet"))
        {
            ReactToPlayerAttack(other.transform.position);
        }
    }

    void ReactToPlayerAttack(Vector2 attackPos)
    {
        float dist = Vector2.Distance(transform.position, attackPos);

        if (dist > dashTriggerRadius || Time.time < nextDashTime)
            return;

        Vector2 dashDir = ((Vector2)transform.position - attackPos).normalized;
        dashDir = new Vector2(dashDir.x, 0).normalized;

        StartCoroutine(DashRoutine(dashDir));
        nextDashTime = Time.time + dashCooldown;
    }

    System.Collections.IEnumerator DashRoutine(Vector2 dir)
    {
        isDashing = true;

        float endTime = Time.time + dashDuration;

        while (Time.time < endTime)
        {
            rb.linearVelocity = new Vector2(dir.x * dashSpeed, rb.linearVelocity.y);
            yield return null;
        }

        isDashing = false;
    }

    // ---------------------------
    // DAMAGE + FLASH
    // ---------------------------
    public void takeDamage(float amount)
    {
        currentHP -= amount;
        StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    System.Collections.IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = origColor;
    }

    // ---------------------------
    // DEBUG VISUALS
    // ---------------------------
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
