using UnityEngine;

public class BossController : MonoBehaviour, IDamage
{
    //different boss fight phases
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

    [Header("Camera")]
    [SerializeField] Unity.Cinemachine.CinemachineCamera cinemachineCamera;
    [SerializeField] Transform cameraLockTarget;
    [SerializeField] Vector3 phase2CameraOffset = new Vector3(0f, 6f, -10f);

    [Header("Phase")]
    [SerializeField] BossPhase currentPhase;

    [Header("Phase1")]
    [SerializeField] GameObject missilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 2f;

    [Header("Phase 1 Follow")]
    [SerializeField] float followOffsetX = 7f;
    [SerializeField] float followSpeed = 3f;

    [Header("Phase 2 Movement")]
    [SerializeField] float phase2MoveSpeed = 2f;
    [SerializeField] float phase2MoveDistance = 6f;
    [Range(0, 100)][SerializeField] float phase2HeightAbovePlayer = 6f;

    [Header("Phase 2 Attack")]
    [SerializeField] GameObject normalMissilePrefab;
    [SerializeField] GameObject redMissilePrefab;
    [SerializeField] Transform phase2MiddleFirePoint;
    [SerializeField] Transform phase2TopFirePoint;
    [SerializeField] Transform phase2BottomFirePoint;
    [SerializeField] float phase2FireRate = 1.2f;
    [SerializeField] float phase2MissileAngleOffset = 15f;

    [Header("Phase 2 Shield")]
    [SerializeField] int shieldHitsNeeded = 3;
    [SerializeField] GameObject shieldVisual;

    [SerializeField] Transform bossMissileTarget;
    Unity.Cinemachine.CinemachineFollow cinemachineFollow;
    float nextFireTime;
    Vector3 phase2StartPos;
    int currentShieldHits;
    bool shieldBroken;

    void Start()
    {
        currentHealth = maxHealth;
        currentPhase = BossPhase.Waiting;

        //automatically assign important references
        AutoAssignReferences();

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
        //move boss to hidden spawn point at start
        if (bossSpawnPoint != null)
        {
            transform.position = bossSpawnPoint.position;
        }
        if (cinemachineCamera != null)
        {
            cinemachineFollow = cinemachineCamera.GetComponent<Unity.Cinemachine.CinemachineFollow>();
        }
    }

    void Update()
    {
        //boss flies into arena during intro
        if (currentPhase == BossPhase.Intro)
        {
            MoveToIntroPosition();
        }

        //phase 1 behaviour
        if (currentPhase == BossPhase.Phase1)
        {
            FollowPlayerPhase1();
            HandlePhase1();
        }
        //phase 2 behaviour
        if (currentPhase == BossPhase.Phase2)
        {
            HandlePhase2();
        }
    }

    //keeps boss near right side of the camera
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
    //controls missiles fire timing
    void HandlePhase1()
    {
        if (Time.time >= nextFireTime)
        {
            ShootMissile();

            nextFireTime = Time.time + fireRate;
        }
    }

    //spawns and launches a missile toward player
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
            missileScript.SetBossTarget(bossMissileTarget);
        }
    }

    //starts intro movement
    public void StartBossIntro()
    {
        currentPhase = BossPhase.Intro;
    }

    //moves boss into arena at start of fight
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

        //start phase 1 once boss reaches position
        if (distance <= 0.1f)
        {
            transform.position = introTargetPosition.position;
            currentPhase = BossPhase.Phase1;
        }
    }

    //regular damage handling
    public void takeDamage(float amount)
    {
        if (currentPhase == BossPhase.Waiting || currentPhase == BossPhase.Intro)
        {
            return;
        }

        //phase 1 only takes reflected missiles damage
        if (currentPhase == BossPhase.Phase1 || currentPhase == BossPhase.Phase2)
        {
            return;
        }


        currentHealth -= amount;

        //change phases based on remaining health
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

    //special damage used only for reflected missiles
    public void takeReflectedDamage(float amount)
    {
        if (currentPhase == BossPhase.Phase1)
        {
            currentHealth -= amount;

            if (currentHealth <= 75f)
            {
                StartPhase2();
            }

            return;
        }

        if (currentPhase == BossPhase.Phase2)
        {
            if (!shieldBroken)
            {
                currentShieldHits++;

                if (currentShieldHits >= shieldHitsNeeded)
                {
                    shieldBroken = true;

                    if (shieldVisual != null)
                    {
                        shieldVisual.SetActive(false);
                    }

                    Debug.Log("Shield Broken!");
                }

                return;
            }

            currentHealth -= amount;

            if (currentHealth <= 0f)
            {
                gamemanager.instance.youWin();
                Destroy(gameObject);
            }
        }
    }

    //automatically finds needed references fro prefab setup
    void AutoAssignReferences()
    {
        if (firePoint == null)
        {
            firePoint = transform.Find("missileFirePoint");
        }
        Transform parent = transform.parent;

        if (parent != null)
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

    void StartPhase2()
    {
        currentPhase = BossPhase.Phase2;

        if (gamemanager.instance != null && gamemanager.instance.player != null)
        {
            Vector3 playerPos = gamemanager.instance.player.transform.position;

            transform.position = new Vector3(
                playerPos.x,
                playerPos.y + phase2HeightAbovePlayer,
                transform.position.z
            );
        }

        phase2StartPos = transform.position;
        nextFireTime = Time.time + 1f;

        currentShieldHits = 0;
        shieldBroken = false;

        if (shieldVisual != null)
        {
            shieldVisual.SetActive(true);
        }

        if (cameraLockTarget != null && gamemanager.instance != null && gamemanager.instance.player != null)
        {
            cameraLockTarget.position = gamemanager.instance.player.transform.position;
        }

        if (cinemachineFollow != null)
        {
            cinemachineFollow.FollowOffset = phase2CameraOffset;
        }

        if (cinemachineCamera != null && cameraLockTarget != null)
        {
            cinemachineCamera.Follow = cameraLockTarget;
        }
    }

    void HandlePhase2()
    {
        MovePhase2();
        HandlePhase2Attack();
    }

    void MovePhase2()
    {
        float moveAmount = Mathf.Sin(Time.time * phase2MoveSpeed) * phase2MoveDistance;

        transform.position = new Vector3(
            phase2StartPos.x + moveAmount,
            phase2StartPos.y,
            transform.position.z
        );
    }

    void HandlePhase2Attack()
    {
        if (Time.time >= nextFireTime)
        {
            ShootPhase2Missiles();
            nextFireTime = Time.time + phase2FireRate;
        }
    }

    void ShootPhase2Missiles()
    {
        ShootPhase2Missile(phase2MiddleFirePoint, 0f, true);
        ShootPhase2Missile(phase2TopFirePoint, phase2MissileAngleOffset, false);
        ShootPhase2Missile(phase2BottomFirePoint, -phase2MissileAngleOffset, false);
    }

    void ShootPhase2Missile(Transform spawnPoint, float angleOffset, bool reflectable)
    {
        if (spawnPoint == null || gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return;
        }

        GameObject prefabToUse = reflectable ? normalMissilePrefab : redMissilePrefab;

        if (prefabToUse == null)
        {
            return;
        }

        Vector2 direction = gamemanager.instance.player.transform.position - spawnPoint.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += angleOffset;

        Vector2 offsetDirection = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        GameObject missile = Instantiate(prefabToUse, spawnPoint.position, Quaternion.identity);

        BossMissile missileScript = missile.GetComponent<BossMissile>();

        if (missileScript != null)
        {
            missileScript.SetDirection(offsetDirection);
            missileScript.SetReflectable(reflectable);
            missileScript.SetBossTarget(bossMissileTarget);
        }
    }
}