using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [Header("Movement")][SerializeField] float moveSpeed = 14f; [SerializeField] float lockOnTime = 0.35f; [SerializeField] float predictionMultiplier = 0.5f; [SerializeField] float rotationOffset = -180f;

    [Header("Damage")]
    [SerializeField] float damage = 10f;


    [Header("Lifetime")]
    [SerializeField] float lifeTime = 5f;

    [Header("Reflect")]
    [SerializeField] Color reflectedColor = Color.cyan;
    [SerializeField] float reflectedLifeTime = 8f;
    [SerializeField] float reflectedSpeedMultiplier = 1.25f;



    [Header("Phase 2")]
    [SerializeField] bool canReflect = true;
    [SerializeField] Color unreflectableColor = Color.red;

    [Header("Boss Hit")]
    [SerializeField] float bossHitRadius = 3f;

    [Header("Effects")]
    [SerializeField] GameObject explostionEffect;


    Collider2D missileCollider;
    Transform bossTarget;
    bool canHitBoss;
    SpriteRenderer spriteRenderer;
    bool isReflected;
    Rigidbody2D rb;
    Vector2 moveDirection;
    bool hasLaunched;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        missileCollider = GetComponent<Collider2D>();
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
            CheckBossHitRadius();
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

        BossMissile otherMissile = collision.gameObject.GetComponent<BossMissile>();

        if (otherMissile != null)
        {
            return;
        }

        // reflected missile damages boss
        if (collision.gameObject.CompareTag("Boss"))
        {
            if (!isReflected || !canHitBoss)
            {
                return;
            }

            BossController boss = collision.gameObject.GetComponent<BossController>();

            if (boss != null)
            {
                boss.takeReflectedDamage(damage);
            }

            Explode();
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

            Explode();
            return;
        }

        // ignore boss collision before reflection
        if (!isReflected && collision.gameObject.CompareTag("Boss"))
        {
            return;
        }

        Explode();
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

        CancelInvoke(nameof(Launch));

        canHitBoss = false;
        Invoke(nameof(EnableBossHit), 0.2f);

        gameObject.layer = LayerMask.NameToLayer("ReflectedMissile");

        Collider2D bossCollider = null;
        GameObject boss = GameObject.FindGameObjectWithTag("Boss");

        if (boss != null)
        {
            bossCollider = boss.GetComponent<Collider2D>();
        }

        if (missileCollider != null && bossCollider != null)
        {
            Physics2D.IgnoreCollision(missileCollider, bossCollider, true);
        }

        Destroy(gameObject, reflectedLifeTime);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = reflectedColor;
        }

        HomeToBoss();
    }

    void EnableBossHit()
    {
        canHitBoss = true;
    }

    //continuously homes reflected missile toward boss
    void HomeToBoss()
    {
        if (bossTarget == null)
        {
            GameObject targetObj = GameObject.FindGameObjectWithTag("BossMissileTarget");

            if (targetObj != null)
            {
                bossTarget = targetObj.transform;
            }
            else
            {
                GameObject boss = GameObject.FindGameObjectWithTag("Boss");

                if (boss != null)
                {
                    bossTarget = boss.transform;
                }
            }
        }

        if (bossTarget == null)
        {
            return;
        }

        Vector2 direction = bossTarget.position - transform.position;

        moveDirection = direction.normalized;

        RotateToDirection(moveDirection);

        if (rb != null)
        {
            rb.linearVelocity =
                moveDirection * moveSpeed * reflectedSpeedMultiplier;
        }
    }

    public bool CanReflect()
    {
        return canReflect;
    }

    public void SetReflectable(bool value)
    {
        canReflect = value;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        Debug.Log("Reflectable: " + canReflect);
        Debug.Log("Sprite Renderer Found: " + spriteRenderer);

        if (spriteRenderer != null)
        {
            if (canReflect)
            {
                spriteRenderer.color = Color.white;
            }
            else
            {
                spriteRenderer.color = unreflectableColor;
            }
        }
    }

    void CheckBossHitRadius()
    {
        if (!canHitBoss)
        {
            return;
        }

        if (bossTarget == null)
        {
            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            bossTarget.position
        );

        if (distance <= bossHitRadius)
        {
            rb.linearVelocity = Vector2.zero;

            GameObject boss = GameObject.FindGameObjectWithTag("Boss");

            if (boss != null)
            {
                BossController bossController = boss.GetComponent<BossController>();

                if (bossController != null)
                {
                    bossController.takeReflectedDamage(damage);
                }
            }

            Explode();
        }
    }

    void Explode()
    {
        if (explostionEffect != null)
        {
            Instantiate(explostionEffect, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }

}