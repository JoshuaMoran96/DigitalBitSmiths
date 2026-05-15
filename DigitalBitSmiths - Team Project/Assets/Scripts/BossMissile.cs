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
        //destroy missile after a set time
        Destroy(gameObject, lifeTime);
        //delay launch so missile can "lock on"
        Invoke(nameof(Launch), lockOnTime);
    }

    //sets initial direction from boss
    public void SetDirection(Vector2 direction)
    {
        moveDirection = direction.normalized;
        RotateToDirection(moveDirection);
    }

    //launches missile after lock on delay
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
        //reflected missiles constantly home toward boss
        if (isReflected)
        {
            HomeToBoss();
            return;
        }
        //predict player movement before missile launches
        if (!hasLaunched && gamemanager.instance != null && gamemanager.instance.player != null)
        {
            Transform player = gamemanager.instance.player.transform;
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();

            float distanceToPlayer = Vector2.Distance(transform.position, player.position);
            float travelTime = distanceToPlayer / moveSpeed;

            Vector2 predictedPosition = player.position;

            //predict where player will move
            if (playerRb != null)
            {
                predictedPosition += playerRb.linearVelocity * travelTime * predictionMultiplier;
            }

            Vector2 direction = predictedPosition - (Vector2)transform.position;

            moveDirection = direction.normalized;

            RotateToDirection(moveDirection);
        }
    }

    //rotates missile to face movement direction
    void RotateToDirection(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle + rotationOffset);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //ignore player bullets so missile can be reflected
        if (collision.gameObject.CompareTag("Bullet"))
        {
            return;
        }

        //ignore collisions with other boss missiles
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

    //converts enemy missile into reflected missile
    public void Reflect()
    {
        if (isReflected)
        {
            return;
        }

        isReflected = true;
        hasLaunched = true;

        //move missile onto reflected layer
        gameObject.layer = LayerMask.NameToLayer("ReflectedMissile");

        CancelInvoke();
        CancelInvoke(nameof(Launch));

        //give reflected missile extra lifetime
        Destroy(gameObject, reflectedLifeTime);

        //change missile color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = reflectedColor;
        }

        HomeToBoss();
    }
    
    //continuously homes reflected missile toward boss
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