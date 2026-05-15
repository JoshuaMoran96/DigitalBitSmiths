using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour, IDamage
{

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] float flashTime = 0.1f;

    Color originalColor;
    [Header("Target")]
    [SerializeField] private Transform player;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float detectionRange = 6f;
    [SerializeField] private float stopDistance = 1f;

    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Damage")]
    [SerializeField] private int touchDamage = 1;
    [SerializeField] float attackDistance = 1.2f;
    [SerializeField] float verticalAttackRange = 1.2f;
    [SerializeField] float damageRate = 1f;

    //timer for repeated damage
    float nextDamageTime;
    private float currentHealth;
    private Rigidbody2D rb;


    private void Start()
    {
        // auto assign sprite renderer if missing
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        //save orignal sprite color
        originalColor = spriteRenderer.color;

        //get rigidbody component
        rb = GetComponent<Rigidbody2D>();

        currentHealth = maxHealth;

    }

    private void FixedUpdate()
    {
        //safety checks
        if (gamemanager.instance == null || gamemanager.instance.player == null || rb == null)
        {
            return;
        }

        //get player from game manager
        player = gamemanager.instance.player.transform;

        //calculate player distance
        float xDistance = Mathf.Abs(transform.position.x - player.position.x);
        float yDistance = Mathf.Abs(transform.position.y - player.position.y);

        //stop and damage player if close enough
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

        // chase player if inside detection range
        if (xDistance <= detectionRange)
        {
            if (player.position.x > transform.position.x)
            {
                rb.linearVelocity = new Vector2(moveSpeed, rb.linearVelocity.y);
            }
            else
            {
                rb.linearVelocity = new Vector2(-moveSpeed, rb.linearVelocity.y);
            }
        }
        else
        {
            //stop moving if player leaves range
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }
    //called when enemy takes damage
    public void takeDamage(float amount)
    {
        currentHealth -= amount;

        StartCoroutine(FlashRed());

        //destroyh enemy when health reaches 0
        if (currentHealth <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateEnemyCount(-1);
            }

            Destroy(gameObject);
        }
    }



    //Visualize detection range in editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }

    //flahes enemy red when hit
    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        spriteRenderer.color = originalColor;
    }

}