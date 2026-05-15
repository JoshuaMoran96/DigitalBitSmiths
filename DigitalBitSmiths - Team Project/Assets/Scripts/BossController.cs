using UnityEngine;

public class BossController : MonoBehaviour, IDamage
{

    public enum BossPhase
    {
        Waiting,
        Intro,
        Phase1,
        Phase2,
        Phase3,
        Phase4
    }
    [Header("Intro")]
    [SerializeField] Transform bossSpawnPoint;
    [SerializeField] Transform introTargetPosition;
    [SerializeField] float introMoveSpeed = 5f;

    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;

    [Header("Phase")]
    [SerializeField] BossPhase currentPhase;

    [Header("Phase1")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 2f;

    [Header("Phase 1 Follow")]
    [SerializeField] float followOffsetX = 7f;
    [SerializeField] float followSpeed = 3f;


    float nextFireTime;

    void Start()
    {
        currentHealth = maxHealth;
        currentPhase = BossPhase.Waiting;

        AutoAssignReferences();

        if (bossSpawnPoint != null)
        {
            transform.position = bossSpawnPoint.position;
        }
    }

    void Update()
    {
        if (currentPhase == BossPhase.Intro)
        {
            MoveToIntroPosition();
        }

        if (currentPhase == BossPhase.Phase1)
        {
            FollowPlayerPhase1();
            HandlePhase1();
        }
    }

    void FollowPlayerPhase1()
    {
        Camera mainCam = Camera.main;

        if (mainCam == null)
        {
            return;
        }

        float screenHalfWidth =
            mainCam.orthographicSize * mainCam.aspect;

        float targetX =
            mainCam.transform.position.x + screenHalfWidth + 10f;

        Vector3 targetPos = new Vector3(
            targetX,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            followSpeed * Time.deltaTime
        );
    }
    void HandlePhase1()
    {
        if (Time.time >= nextFireTime)
        {
            ShootMissile();

            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootMissile()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return;
        }

        Vector2 direction =
            gamemanager.instance.player.transform.position - firePoint.position;

        GameObject missile =
            Instantiate(missilePrefab, firePoint.position, Quaternion.identity);

        BossMissile missileScript =
            missile.GetComponent<BossMissile>();

        if (missileScript != null)
        {
            missileScript.SetDirection(direction);
        }
    }

    public void StartBossIntro()
    {
        currentPhase = BossPhase.Intro;
    }

    void MoveToIntroPosition()
    {
        if (introTargetPosition == null)
        {
            currentPhase = BossPhase.Phase1;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            introTargetPosition.position,
            introMoveSpeed * Time.deltaTime
            );

        float distance = Vector3.Distance(transform.position, introTargetPosition.position);

        if (distance <= 0.1f)
        {
            transform.position = introTargetPosition.position;
            currentPhase = BossPhase.Phase1;
        }
    }

    public void takeDamage(float amount)
    {
        if (currentPhase == BossPhase.Waiting || currentPhase == BossPhase.Intro)
        {
            return;
        }

        if (currentPhase == BossPhase.Phase1)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
        else if (currentHealth <= 25f)
        {
            currentPhase = BossPhase.Phase4;
        }
        else if (currentHealth <= 50f)
        {
            currentPhase = BossPhase.Phase3;
        }
        else if (currentHealth <= 75f)
        {
            currentPhase = BossPhase.Phase2;
        }
    }

    public void takeReflectedDamage(float amount)
    {
        if (currentPhase != BossPhase.Phase1)
        {
            return;
        }

        currentHealth -= amount;

        if (currentHealth <= 75f)
        {
            currentPhase = BossPhase.Phase2;
        }
    }

    void AutoAssignReferences()
    {
        if (firePoint == null)
        {
            firePoint = transform.Find("missileFirePoint");
        }
        Transform parent = transform.parent;

        if(parent != null)
        {
            if (bossSpawnPoint == null)
            {
                bossSpawnPoint = parent.Find("BossSpawnPoint");
            }

            if (introTargetPosition == null)
            {
                introTargetPosition = parent.Find("BossPhase1Position");
            }
        }
    }
}