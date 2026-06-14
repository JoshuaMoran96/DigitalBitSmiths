using System.Collections;
using UnityEngine;
using UnityEngine.AI;



public class crashTestDummy : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Transform target;

    [Header("----- AI STATS -----")]
    public float chaseDist = 10f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 12f;
    [SerializeField] float jumpObstCheck = 1f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundRayLength = 0.5f;

    [Header("----- Jump Settings -----")]
    [SerializeField] int maxJumpCount = 2;
    int jumpCount;
    bool isGrounded;

    [Header("----- Dash Reaction -----")]
    [SerializeField] float dashTriggerRadius = 3f;
    [SerializeField] float dashCooldown = 2f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] float dashDuration = 0.15f;


    float nextDashTime = 0f;
    bool canDash = true;
    bool isDashing;

    [Header("----- Health -----")]
    [SerializeField] float currentHP = 100f;

    Color origColor;

    //Adding ladder compont movement
    private EnemyLadderMovement ladderController;
    


    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (capsuleCollider == null)
            capsuleCollider = GetComponent<CapsuleCollider2D>();

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (target == null && gamemanager.instance != null)
            target = gamemanager.instance.player.transform;

        origColor = spriteRenderer.color;

        jumpCount = maxJumpCount;

        if (gamemanager.instance != null && gamemanager.instance.playerScript != null)
        {
            currentHP = gamemanager.instance.playerScript.currentHP * 1.25f;
        }

        //ladder application
        ladderController = GetComponent<EnemyLadderMovement>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }

        // If the enemy touches a ladder and needs to go up to reach the player
        if (ladderController.IsOnLadder() && target.position.y > transform.position.y)
        {
            // Tell the script to move UP (1f)
            ladderController.StartClimbing(1f);
        }

        GroundCheck();
        HandleMovement();
        HandleJumping();

    }

    //logic function check for ground
    void GroundCheck()
    {
        if (capsuleCollider == null)
            return;

        Vector2 bottom = new Vector2(transform.position.x, capsuleCollider.bounds.min.y);

        isGrounded = Physics2D.Raycast(bottom, Vector2.down, groundRayLength, groundMask);

        if (isGrounded && rb.linearVelocity.y <= 0.5f)
        {
            jumpCount = maxJumpCount;
        }
    }

    //logic for the movements
    void HandleMovement()
    {
        if (target == null || rb == null)
            return;

        float dir = target.position.x - transform.position.x;
        float distance = Mathf.Abs(dir);
        float direction = Mathf.Sign(dir);

        if (distance > chaseDist)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        Vector2 chest = (Vector2)transform.position + new Vector2(0, 0.5f);
        bool wallAhead = Physics2D.Raycast(chest, Vector2.right * direction, 0.5f, groundMask);

        if (wallAhead)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = direction < 0;
        }
    }

    //setup logic for jumping

    void HandleJumping()
    {
        if (target == null)
            return;

        float dir = Mathf.Sign(target.position.x - transform.position.x);

        Vector2 feet = (Vector2)transform.position + new Vector2(0, -0.2f);
        Vector2 chest = (Vector2)transform.position + new Vector2(0, 0.5f);

        // 1. Wall in front jump
        bool wallLow = Physics2D.Raycast(feet, Vector2.right * dir, jumpObstCheck, groundMask);
        // 2. Wall reqires  jump
        bool wallHigh = Physics2D.Raycast(chest, Vector2.right * dir, jumpObstCheck, groundMask);

        // 3. Gap ahead jump
        Vector2 gapCheckOrigin = feet + new Vector2(dir * 1.0f, 0);
        bool gapAhead = !Physics2D.Raycast(gapCheckOrigin, Vector2.down, 1.5f, groundMask);


        // Only jump if there is an actual wall or gap AND the enemy is moving toward it
        float distanceToTarget = Mathf.Abs(target.position.x - transform.position.x);
        if ((wallLow || wallHigh || gapAhead) && isGrounded)
        {
            AIJump();
        }
    }

    //Jump ability mimicing playerController
    void AIJump()
    {
        if (isGrounded && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpCount--;
        }
    }


    //Dash capability mimicing players 
    void AIDash()
    {
        if (!canDash || target == null || rb == null)
            return;

        canDash = false;
        isDashing = true;

        float dir = Mathf.Sign(target.position.x - transform.position.x);

        rb.linearVelocity = new Vector2(dir * dashSpeed, 0f);

        // Save gravity scale to prevent falling mid-dash
        float origGravity = rb.gravityScale;
        rb.gravityScale = 0f;

        // Use a small local function or Coroutine instead of Invoke to restore gravity
        StartCoroutine(DashDurationRoutine(origGravity));
        Invoke(nameof(ResetDash), dashCooldown);

    }

//working on dash issue
IEnumerator DashDurationRoutine(float originalGravity)
    {
        yield return new WaitForSeconds(dashDuration);
        rb.gravityScale = originalGravity;
        isDashing = false;
    }

    //End the dash action
    void EndDash()
    {
        isDashing = false;
    }

    void ResetDash()
    {
        canDash = true;
    }

    //Trigger Collider for Player's attack detection. Will allow Superior Joe to dash when he senses an attack, rather than at random times.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Don't process trigger checks if already dashing
        if (isDashing) return;

        // If a player attack enters the trigger
        if (other.CompareTag("Bullet"))
        {
            // ONLY react if the dash cooldown has passed
            if (Time.time >= nextDashTime)
            {
                OnPlayerAttack(other.transform.position);
            }
        }
    }

        public void takeDamage(float amount)
    {
        // If the enemy is dashing, they are invincible and take no damage
        if (isDashing) return;

        currentHP -= amount;
        Debug.Log("Current HP: " + currentHP);
        StartCoroutine(FlashRed());

        if (currentHP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = origColor;
    }

    public void OnPlayerAttack(Vector2 attackPos)
    {
        // Update the cooldown timer BEFORE dashing to block same-frame repeats
        nextDashTime = Time.time + dashCooldown;
        AIDash();
        //float dist = Vector2.Distance(transform.position, attackPos);

        //if (dist <= dashTriggerRadius && Time.time >= nextDashTime)
        //{
        //    AIDash();
        //    nextDashTime = Time.time + dashCooldown;
        //}
    }
}
