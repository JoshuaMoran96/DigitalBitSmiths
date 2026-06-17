using System.Collections;
using UnityEngine;

public class playerMelee : MonoBehaviour
{
    //setting up to Give AJ the ability to attack with the right button mouse click and attack with the wrench

    [Header("Melee Settings")]
    [SerializeField] Transform meleePoint;
    [SerializeField] float meleeRange = 1f;
    [SerializeField] float meleeDamage = 5f;
    [SerializeField] float knockbackForce = 5f;
    [SerializeField] float meleeCooldown = 0.5f;
    [SerializeField] LayerMask enemyLayer;
  

    [Header("Visual")]
    [SerializeField] GameObject wrenchVisual;
    [SerializeField] SpriteRenderer wrenchRenderer;
    [SerializeField] float visualTime = 0.5f;

    [Header("Facing")]
    [SerializeField] playerController playerControllerScript;
    [SerializeField] float meleePointXOffset = 1.1f;
    [SerializeField] Vector2 rightFacingOffset = new Vector2(0.7f, -0.6f);
    [SerializeField] Vector2 leftFacingOffset = new Vector2(-0.7f, -0.6f);
    [SerializeField] bool flipWrenchSprite = true;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip wrenchSwingSound;
    [SerializeField] float wrenchSwingVolume = 1f;

    float nextMeleeTime;
    Coroutine visualRoutine;


    void Start()
    {
        Debug.Log("playerMelee script started");

        if (wrenchVisual != null && wrenchRenderer == null)
        {
            wrenchRenderer = wrenchVisual.GetComponent<SpriteRenderer>();
        }

        if (playerControllerScript == null)
        {
            playerControllerScript = GetComponent<playerController>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        HideWrenchVisual();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click detected by playerMelee");
        }

        if (Time.time < nextMeleeTime)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            MeleeAttack();
            nextMeleeTime = Time.time + meleeCooldown;
        }
    }

    // adding a direction flip for the weapon
    void UpdateMeleeDirection()
    {
        if (meleePoint == null || playerControllerScript == null)
        {
            return;
        }

        if (playerControllerScript.isFacingRight)
        {
            meleePoint.localPosition = rightFacingOffset;

            if (wrenchRenderer != null && flipWrenchSprite)
            {
                wrenchRenderer.flipX = false;
            }
        }
        else
        {
            meleePoint.localPosition = leftFacingOffset;

            if (wrenchRenderer != null && flipWrenchSprite)
            {
                wrenchRenderer.flipX = true;
            }
        }
    }

    void MeleeAttack()
    {
        Debug.Log("Melee attack triggered");

        UpdateMeleeDirection();
        ShowWrenchVisual();
        PlayWrenchSound();

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            meleePoint.position,
            meleeRange,
            enemyLayer
        );

        foreach (Collider2D hit in hits)
        {
            IDamage dmg = hit.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(meleeDamage);
            }

            // Restructured knockback to work with EnemyAI script
            float direction = Mathf.Sign(hit.transform.position.x - transform.position.x);
            Vector2 knockbackDir = new Vector2(direction, 0.15f).normalized;

            EnemyAI enemyAI = hit.GetComponent<EnemyAI>();

            if (enemyAI != null)
            {
                enemyAI.ApplyKnockback(knockbackDir, knockbackForce);
            }
            else
            {
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();

                if (enemyRb != null)
                {
                    enemyRb.linearVelocity = Vector2.zero;
                    enemyRb.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
                }
            }
        }
    }

    void ShowWrenchVisual()
    {
        Debug.Log("Wrench visual triggered");

        if (wrenchRenderer == null)
        {
            Debug.LogWarning("Wrench Renderer is not assigned.");
            return;
        }

        wrenchRenderer.enabled = true;

        CancelInvoke(nameof(HideWrenchVisual));
        Invoke(nameof(HideWrenchVisual), visualTime);
    }

    void HideWrenchVisual()
    {
        if (wrenchRenderer != null)
        {
            wrenchRenderer.enabled = false;
        }
    }

    void PlayWrenchSound()
    {
        if (audioSource != null && wrenchSwingSound != null)
        {
            audioSource.PlayOneShot(wrenchSwingSound, wrenchSwingVolume);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (meleePoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(meleePoint.position, meleeRange);
    }
}
