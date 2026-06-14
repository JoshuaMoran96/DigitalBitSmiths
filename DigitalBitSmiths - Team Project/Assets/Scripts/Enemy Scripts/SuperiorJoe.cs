using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.InputManagerEntry;

public class SuperiorJoe : MonoBehaviour, IDamage
{
    public playerController controller;
    public Transform target;

    [Header("----- AI STATS -----")]
    public float chaseDist = 10f;
    public float jumpObstCheck = 1f;
    public LayerMask groundMask;

    [Header("----- Dash Reaction -----")]
    public float dashTriggerRadius = 3f;
    public float dashCooldown = 2f;
    private float nextDashTime = 0f;

    public SpriteRenderer spriteRenderer;
    public Color origColor;
    public float currentHP = 100f;
    

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        origColor = spriteRenderer.color;

        controller.jumpCount = 2;

        currentHP += gamemanager.instance.playerScript.currentHP * (1f + .25f);
        
    }
    // Update is called once per frame
    void Update()
    {
        if (controller.isDashing)
        {
            return;
        }

        HandleMovement();
        HandleJumping();
        
    }

    //Trigger Collider for Player's attack detection. Will allow Superior Joe to dash when he senses an attack, rather than at random times.
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If a player attack enters the trigger
        if (other.CompareTag("Bullet"))
        {
            OnPlayerAttack(other.transform.position);
        }
    }

    void HandleMovement()
    {
        float dir = target.position.x - transform.position.x;
        float moveDir = Mathf.Abs(dir);

        if(Mathf.Abs(dir) > chaseDist)
        {
            controller.SetAIMovement(0);
        }

        Vector2 chest = (Vector2)transform.position + new Vector2(0, 0.5f);
        bool wallAhead = Physics2D.Raycast(chest, Vector2.right * moveDir, 0.5f, groundMask);
        
        if(wallAhead)
        {
            controller.SetAIMovement(0);
        }

        controller.SetAIMovement(Mathf.Sign(dir));
    }

    void HandleJumping()
    {
        
        float dir = controller.isFacingRight ? 1 : -1;

        Vector2 feet = (Vector2)transform.position + new Vector2(0, -0.2f);
        Vector2 chest = (Vector2)transform.position + new Vector2(0, 0.5f);

        // 1. Wall in front → jump
        bool wallLow = Physics2D.Raycast(feet, Vector2.right * dir, jumpObstCheck, groundMask);
        bool wallHigh = Physics2D.Raycast(chest, Vector2.right * dir, jumpObstCheck, groundMask);

        // 3. Gap ahead → jump
        bool gapAhead = !Physics2D.Raycast(feet + new Vector2(dir * 0.5f, 0), Vector2.down, 1f, groundMask);

        if ((wallLow || wallHigh || gapAhead) && controller.isGrounded)
        {
            controller.AIJump();
        }
    }

    public void takeDamage(float amount)
    {
        currentHP -= amount;
        Debug.Log("Current HP: " + currentHP);
        StartCoroutine(FlashRed());

        if(currentHP <= 0)
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
        float dist = Vector2.Distance(transform.position, attackPos);
        Vector2 selfPos = (Vector2)transform.position;

        Vector2 dashDir = (selfPos - attackPos).normalized;

        dashDir = new Vector2(dashDir.x, 0).normalized;

        if (dist <= dashTriggerRadius && Time.time >= nextDashTime)
        {
            controller.AIDash(dashDir);
            nextDashTime = Time.time + dashCooldown;
        }
    }
}
