using UnityEngine;
using System.Collections;

public class SuperiorJoe : MonoBehaviour, IDamage
{
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    [Header("----- Target -----")]
    public Transform target;

    [Header("----- Movement -----")]
    public float moveSpeed = 6f;
    public float chaseDistance = 12f;
    public float stopDistance = 1.5f;
    public Transform lastKnownPos;
    public bool isChasing;

    [Header("----- Jumping -----")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundMask;
    public float jumpForce = 12f;
    public float obstacleCheckDistance = 0.75f;
    public int jumpCount;

    private bool isGrounded;

    [Header("----- Dash -----")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.15f;
    public float iFrameDuration = .35f;
    public float dashCooldown = 1.5f;
    private float nextDashTime = 0f;
    private bool isDashing;

    [Header("----- Dash Reaction -----")]
    public float dashTriggerRadius = 3f;

    [Header("----- Health -----")]
    public float maxHP;
    private float currentHP;
    private Color origColor;

    [Header("----- Shooting -----")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float burstRange = 10f;
    public float barrageRange = 4f;

    public int burstCount = 5;
    public float burstDelay = 0.12f;
    public float burstCD = 1.5f;

    public int barrageCount = 10;
    public float barrageDelay = 0.2f;
    public float barrageCooldown = 2.5f;

    [HideInInspector] public float nextAttackTime = 0f;

    private StateMachine currentState;

    public void ChangeState(StateMachine newState)
    {
        currentState?.Exit(this);
        currentState = newState;
        currentState.Enter(this);
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Superior Joe now uses a rigged child, so SpriteRenderer may not be on the root.
        //REWORKED to overcome a merge error from update and the Animation for SJ
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null || spriteRenderer.sprite == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (spriteRenderer != null)
        {
            origColor = spriteRenderer.color;
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
        }

        if (gamemanager.instance != null && gamemanager.instance.playerScript != null)
        {
            maxHP = gamemanager.instance.playerScript.currentHP * 2.0f;

            if (target == null)
            {
                target = gamemanager.instance.playerScript.transform;
            }
        }
        else
        {
            maxHP = 200f;
        }

        currentHP = maxHP;

        ChangeState(new IdleState());

        Physics2D.IgnoreLayerCollision(gameObject.layer, LayerMask.NameToLayer("EnemyBullet"), true);
    }

    void Update()
    {
        GroundCheck();

        RotateFirePoint();

        currentState?.Update(this);
    }

    // ---------------
    // GROUND CHECK
    // ---------------
    void GroundCheck()
    {
        if (groundCheck == null)
        {
            isGrounded = false;
            return;
        }

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundMask
        );

        if (isGrounded)
        {
            jumpCount = 2;
        }
    }

    // ----------------
    // MOVEMENT
    // ----------------
    public void HandleMovement()
    {
        if (target == null || rb == null)
        {
            return;
        }

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

        // Do not flip the root transform here.
        // SuperiorJoeAnimations handles the rigged visual flip.
        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    // ---------------
    // JUMPING
    // ---------------
    public void HandleJumping()
    {
        if (target == null)
        {
            return;
        }

        float dir = Mathf.Sign(target.position.x - transform.position.x);

        Vector2 feet = (Vector2)transform.position + new Vector2(0, -0.2f);
        Vector2 chest = (Vector2)transform.position + new Vector2(0, 0.5f);

        bool wallLow = Physics2D.Raycast(feet, Vector2.right * dir, obstacleCheckDistance, groundMask);
        bool wallHigh = Physics2D.Raycast(chest, Vector2.right * dir, obstacleCheckDistance, groundMask);

        bool gapAhead = !Physics2D.Raycast(feet + new Vector2(dir * 0.5f, 0), Vector2.down, 1f, groundMask);

        if ((wallLow || wallHigh || gapAhead) && isGrounded && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount--;
        }

        if ((wallLow || wallHigh || gapAhead) && !isGrounded && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount--;
        }
    }

    // ---------------
    // FIRING
    // ---------------
    public void FireBullet()
    {
        if (target == null || firePoint == null || bulletPrefab == null)
        {
            return;
        }

        Vector2 dir = (target.position - firePoint.position).normalized;

        GameObject b = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        if (b.TryGetComponent(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = dir * 12f;
        }
    }

    // ---------------
    // DASH REACTION + IFRAMES
    // ---------------
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
        {
            return;
        }

        Vector2 dashDir = ((Vector2)transform.position - attackPos).normalized;
        dashDir = new Vector2(dashDir.x, 0).normalized;

        StartCoroutine(DashRoutine(dashDir));
        nextDashTime = Time.time + dashCooldown;
    }

    IEnumerator DashRoutine(Vector2 dir)
    {
        isDashing = true;

        StartCoroutine(iFrames());

        float endTime = Time.time + dashDuration;

        while (Time.time < endTime)
        {
            rb.linearVelocity = new Vector2(dir.x * dashSpeed, rb.linearVelocity.y);
            yield return null;
        }

        isDashing = false;
    }

    // -----------------
    // DAMAGE + FLASH
    // -----------------
    public void takeDamage(float amount)
    {
        currentHP -= amount;
        StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            PlayerPrefs.SetInt("SuperiorJoeDefeated", 1);
            PlayerPrefs.Save();

            Debug.Log("Superior Joe defeated. Level 5 unlocked.");

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

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = origColor;
    }

    IEnumerator iFrames()
    {
        int playerBulletLayer = LayerMask.NameToLayer("Bullet");

        Physics2D.IgnoreLayerCollision(gameObject.layer, playerBulletLayer, true);

        SetOpacity(0.25f);

        yield return new WaitForSeconds(iFrameDuration);

        SetOpacity(1.0f);

        Physics2D.IgnoreLayerCollision(gameObject.layer, playerBulletLayer, false);
    }

    public void SetOpacity(float alpha)
    {
        if (spriteRenderer == null)
        {
            return;
        }

        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(alpha);
        spriteRenderer.color = color;
    }

    // ----------------
    // DEBUG VISUALS
    // ----------------
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

    void RotateFirePoint()
    {
        if (firePoint == null || target == null)
        {
            return;
        }

        Vector2 dir = (target.position - firePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }
}
