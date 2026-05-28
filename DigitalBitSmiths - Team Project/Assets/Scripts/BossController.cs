using Unity.VisualScripting;
using UnityEngine;
using Unity.Cinemachine;

public class BossController : MonoBehaviour, IDamage
{//different boss fight phases
 public enum BossPhase{Waiting,Intro,Phase1,Phase2,Phase3,Phase4}
    [Header("Intro")]
    [SerializeField] Transform bossSpawnPoint;
    [SerializeField] Transform introTargetPosition;
    [SerializeField] float introMoveSpeed = 5f;

    [Header("Health")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] BossHealthBar bossHealthBar;

    [Header("Phase")]
    [SerializeField] BossPhase currentPhase;

    [Header("Phase1")]
    [SerializeField] GameObject normalMissilePrefab;
    [SerializeField] GameObject redMissilePrefab;
    [SerializeField] Transform firePoint;
    [SerializeField] float fireRate = 2f;

    [Header("Phase 1 Follow")]
    [SerializeField] float followOffsetX = 7f;
    [SerializeField] float followSpeed = 3f;

    [Header("Phase 2 Movement")]
    [SerializeField] float phase2MoveSpeed = 4f;
    [SerializeField] float phase2MoveDistance = 6f;
    [SerializeField] float phase2HeightAbovePlayer = 6f;
    [SerializeField] float phase2StartXOffset = 0f;

    [Header("Phase 2 fire points")]
    [SerializeField] Transform phase2OuterRightFirePoint;
    [SerializeField] Transform phase2InnerRightFirePoint;
    [SerializeField] Transform phase2OuterLeftFirePoint;
    [SerializeField] Transform phase2InnerLeftFirePoint;


    [Header("Phase 2 Shield")]
    [SerializeField] int shieldHitsNeeded = 3;
    [SerializeField] GameObject shieldVisual;
    [SerializeField] Sprite crackedShieldSprite;
    [SerializeField] SpriteRenderer shieldRenderer;
    [SerializeField] BossShieldBar bossShieldBar;

    [Header("Camera")]
    [SerializeField] CinemachineCamera cinemachineCamera;
    [SerializeField] Transform cameraLockTarget;

    [Header("Effects")]
    [SerializeField] GameObject explostionEffect;


    Camera mainCam;
    CinemachineFollow cinemachineFollow;
    int currentShieldHits;
    bool shieldBroken;
    Vector3 phase2StartPos;
    bool movingRightPhase2 = true;
    float nextFireTime;

    void Start()
    {
        mainCam = Camera.main;
        currentHealth = maxHealth;
        currentPhase = BossPhase.Waiting;

        AutoAssignReferences();
        if (bossShieldBar != null)
        {
            bossShieldBar.UpdateShieldBar(0, shieldHitsNeeded);
        }

        if (bossHealthBar != null)
        {
            bossHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (cinemachineCamera != null)
        {
            cinemachineFollow =
                cinemachineCamera.GetComponent<CinemachineFollow>();

            if (cinemachineFollow == null)
            {
                cinemachineFollow =
                    cinemachineCamera.GetComponentInChildren<CinemachineFollow>();
            }
        }

        if (bossSpawnPoint != null)
        {
            transform.position = bossSpawnPoint.position;
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
            Instantiate(normalMissilePrefab, firePoint.position, Quaternion.identity);

        BossMissile missileScript =
            missile.GetComponent<BossMissile>();

        if (missileScript != null)
        {
            missileScript.SetDirection(direction);
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
            UpdateBossHealthUI();
            BossDeath();
        }
        else if (currentHealth <= 25f)
        {
            UpdateBossHealthUI();
            currentPhase = BossPhase.Phase4;
        }
        else if (currentHealth <= 50f)
        {
            UpdateBossHealthUI();
            currentPhase = BossPhase.Phase3;
        }
        else if (currentHealth <= 75f)
        {
            UpdateBossHealthUI();
            currentPhase = BossPhase.Phase2;
        }
    }

    //special damage used only for reflected missiles
    public void takeReflectedDamage(float amount)
    {
        if (currentPhase == BossPhase.Phase1)
        {
            currentHealth -= amount;
            UpdateBossHealthUI();

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

                UpdateShieldVisual();

                if (bossShieldBar != null)
                {
                    bossShieldBar.UpdateShieldBar(
                        currentShieldHits,
                        shieldHitsNeeded
                    );
                }

                if (currentShieldHits >= shieldHitsNeeded)
                {
                    shieldBroken = true;

                    if (bossShieldBar != null)
                    {
                        bossShieldBar.gameObject.SetActive(false);
                    }

                    Debug.Log("Shield Broken!");
                }

                return;
            }

            currentHealth -= amount;
            UpdateBossHealthUI();

            if (currentHealth <= 0f)
            {
                BossDeath();
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

    void HandlePhase2()
    {
        MovePhase2();
        HandlePhase2Attacks();
    }


    void MovePhase2()
    {
        float leftBound = phase2StartPos.x - phase2MoveDistance;
        float rightBound = phase2StartPos.x + phase2MoveDistance;

        Vector3 pos = transform.position;

        if (movingRightPhase2)
        {
            pos.x += phase2MoveSpeed * Time.deltaTime;

            if (pos.x >= rightBound)
            {
                movingRightPhase2 = false;
            }
        }
        else
        {
            pos.x -= phase2MoveSpeed * Time.deltaTime;
            if (pos.x <= leftBound)
            {
                movingRightPhase2 = true;
            }
        }

        transform.position = pos;
    }


    void HandlePhase2Attacks()
    {
        if (Time.time >= nextFireTime)
        {
            ShootPhase2Missiles();
            nextFireTime = Time.time + fireRate;
        }
    }

    void ShootPhase2Missiles()
    {
        Debug.Log("Outer Left: " + phase2OuterLeftFirePoint);
        Debug.Log("Outer Right: " + phase2OuterRightFirePoint);
        Debug.Log("Inner Left: " + phase2InnerLeftFirePoint);
        Debug.Log("Inner Right: " + phase2InnerRightFirePoint);

        ShootMissileWithOffset(phase2OuterLeftFirePoint, 0f, true);
        ShootMissileWithOffset(phase2OuterRightFirePoint, 20f, false);
        ShootMissileWithOffset(phase2InnerLeftFirePoint, -20f, false);
        ShootMissileWithOffset(phase2InnerRightFirePoint, -20f, true);
    }

    void ShootMissileWithOffset(Transform spawnPoint, float angleOffset, bool reflectable)
    {
        if (spawnPoint == null || gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return;
        }

        Vector2 direction =
            gamemanager.instance.player.transform.position - spawnPoint.position;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        angle += angleOffset;

        Vector2 offsetDirection = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad)
        );

        GameObject prefabToUse =
            reflectable ? normalMissilePrefab : redMissilePrefab;

        GameObject missile = Instantiate(
            prefabToUse,
            spawnPoint.position,
            Quaternion.identity
        );
        missile.transform.SetParent(null);

        BossMissile missileScript = missile.GetComponent<BossMissile>();

        if (missileScript != null)
        {
            missileScript.SetDirection(offsetDirection);
            missileScript.SetReflectable(reflectable);
        }
    }

    void UpdateShieldVisual()
    {
        if (currentShieldHits == 1 && shieldRenderer != null && crackedShieldSprite != null)
        {
            shieldRenderer.sprite = crackedShieldSprite;
        }
        if (currentShieldHits >= shieldHitsNeeded && shieldVisual != null)
        {
            shieldVisual.SetActive(false);
        }
    }

    void StartPhase2()
    {
        currentPhase = BossPhase.Phase2;

        if (gamemanager.instance != null &&
            gamemanager.instance.player != null)
        {
            Vector3 playerPos =
                gamemanager.instance.player.transform.position;

            transform.position = new Vector3(
                playerPos.x + phase2StartXOffset,
                playerPos.y + phase2HeightAbovePlayer,
                transform.position.z
            );

            if (cameraLockTarget != null)
            {
                cameraLockTarget.position = playerPos;
            }
        }

        phase2StartPos = transform.position;

        if (cinemachineFollow != null)
        {
            cinemachineFollow.FollowOffset = new Vector3(0f, 6f, -10f);
        }

        if (cinemachineCamera != null && cameraLockTarget != null)
        {
            cinemachineCamera.Follow = cameraLockTarget;
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

    void BossDeath()
    {
        Debug.Log("Boss Death Triggered");

        if (gamemanager.instance != null)
        {
            gamemanager.instance.youWin();
        }
        Explode();
    }

    void UpdateBossHealthUI()
    {
        if (bossHealthBar != null)
        {
            bossHealthBar.UpdateHealthBar(currentHealth, maxHealth);
        }
    }
}