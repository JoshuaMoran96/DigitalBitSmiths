using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 14f;
    [SerializeField] float lockOnTime = 0.35f;
    [SerializeField] float predictionMultiplier = 0.5f;
    [SerializeField] float rotationOffset = -180f;

    [Header("Damage")]
    [SerializeField] float damage = 10f;


    [Header("Lifetime")]
    [SerializeField] float lifeTime = 5f;

    [Header("Reflect")]
    [SerializeField] Color reflectedColor = Color.cyan;
    [SerializeField] float reflectedLifeTime = 8f;
    [SerializeField] float reflectedSpeedMultiplier = 1.25f;

    SpriteRenderer spriteRenderer;
    bool isReflected;
    Rigidbody2D rb;
    Vector2 moveDirection;
    bool hasLaunched;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
        Invoke(nameof(Launch), lockOnTime);
    }

    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        RotateToDirection(moveDirection);
    }

    void Launch()
    {
        hasLaunched = true;

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed;
        }
    }

    void Update()
    {
        if (isReflected)
        {
            HomeToBoss();
            return;
        }
        if (!hasLaunched && gamemanager.instance != null && gamemanager.instance.player != null)
        {
            Transform player = gamemanager.instance.player.transform;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            float travelTime = distanceToPlayer / moveSpeed;

            Vector2 predictedPosition = player.position;

            if (playerRb != null)
            {
                predictedPosition += playerRb.linearVelocity * travelTime * predictionMultiplier;
            }

            Vector2 direction = predictedPosition - (Vector2)transform.position;

            moveDirection = direction.normalized;

            RotateToDirection(moveDirection);
        }
    }

    void RotateToDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ignore player bullets so they can reflect the missile without destroying it
        if (collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }

        if (collision.gameObject.GetComponent<BossMissile>() != null)
        {
            return;
        }
        // reflected missile damages boss
        if (isReflected && collision.gameObject.CompareTag("Boss"))
        {
            BossController boss = collision.gameObject.GetComponent<BossController>();

            if (boss != null)
            {
                boss.takeReflectedDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // normal missile damages player
        if (!isReflected && collision.gameObject.CompareTag("Player"))
        {
            IDamage dmg = collision.gameObject.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(damage);
            }

            Destroy(gameObject);
            return;
        }

        // ignore boss collision before reflection
        if (!isReflected && collision.gameObject.CompareTag("Boss"))
        {
            return;
        }

        Destroy(gameObject);
    }

    public void Reflect()
    {
        if (isReflected)
        {
            return;
        }

        isReflected = true;
        hasLaunched = true;

        gameObject.layer = LayerMask.NameToLayer("ReflectedMissile");

        CancelInvoke();
        CancelInvoke(nameof(Launch));

        Destroy(gameObject, reflectedLifeTime);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = reflectedColor;
        }

        HomeToBoss();
    }

    void HomeToBoss()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        if (boss == null)
        {
            return;
        }

        Vector2 direction = boss.transform.position - transform.position;

         moveDirection = direction.normalized;

        RotateToDirection(moveDirection);

        if (rb != null)
        {
            rb.linearVelocity = moveDirection * moveSpeed * reflectedSpeedMultiplier;
        }
    }
}