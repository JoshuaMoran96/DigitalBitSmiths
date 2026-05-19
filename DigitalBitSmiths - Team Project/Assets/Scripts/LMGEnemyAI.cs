using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class LMGEnemyAI : MonoBehaviour, IDamage
{

    [Header("References")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] GameObject muzzleFlash;
    [SerializeField] EnemyHealthBar healthBar;

    [Header("Health")]
    [SerializeField] float maxHealth = 40f;
    [SerializeField] float currentHealth;

    [Header("Shooting")]
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float bulletSpread = 8f;
    [SerializeField] float burstDuration = 2.5f;
    [SerializeField] float overheatCooldown = 2f;

    [Header("Damage Feedback")]
    [SerializeField] float flashTime = 0.1f;

    [Header("Weak Spot")]
    [SerializeField] Transform weakSpotCheck;
    [SerializeField] float weakSpotAllowedDistance = 1.5f;

    [Header("Heaet")]
    [SerializeField] SpriteRenderer gunGlow;
    [SerializeField] Color coldColor = new Color(1f, 0f, 0f, 0f);
    [SerializeField] Color hotColor = new Color(1f, 0f, 0f, 0.8f);
    [SerializeField] float currentHeat;
    [SerializeField] float maxHeat = 100f;
    [SerializeField] float heatPerShot = 5f;
    [SerializeField] float coolRate = 25f;

    bool playerInRange;
    bool isFiring;
    bool isOverheated;

    float nextFireTime;
    float burstEndTime;
    Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (healthBar == null)
        {
            healthBar = GetComponentInChildren<EnemyHealthBar>();
        }

        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        if (muzzleFlash != null)
        {
            muzzleFlash.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFiring)
        {
            currentHeat -= coolRate * Time.deltaTime;
            currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
            UpdateGunGlow();
        }

        if (!playerInRange || isOverheated)
        {
            return;
        }

        if (!isFiring)
        {
            StartBurst();
        }

        if (isFiring)
        {
            FireBurst();
        }
    }

    void StartBurst()
    {
        isFiring = true;
        burstEndTime = Time.time + burstDuration;
    }

    void FireBurst()
    {
        if (Time.time >= burstEndTime)
        {

            StartCoroutine(Overheat());
            return;
        }

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            return;
        }

        currentHeat += heatPerShot;
        currentHeat = Mathf.Clamp(currentHeat, 0f, maxHeat);
        UpdateGunGlow();
        float spread = Random.Range(-bulletSpread, bulletSpread);
        Quaternion rotation = firePoint.rotation * Quaternion.Euler(0f, 0f, 180f +  spread);
        Instantiate(bulletPrefab, firePoint.position, rotation);
        if (muzzleFlash != null)
        {
            StartCoroutine(ShowMuzzleFlash());
        }
    }

    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
        yield return new WaitForSeconds(0.04f);
        muzzleFlash.SetActive(false);
    }

    IEnumerator Overheat()
    {
        isFiring = false;
        isOverheated = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }

        yield return new WaitForSeconds(overheatCooldown);

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }

        isOverheated = false;
    }

    public void takeDamage(float amount)
    {
        if (!CanTakeDamageFromPlayer())
        {
            return;
        }

        currentHealth -= amount;

        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(currentHealth, maxHealth);
        }

        StartCoroutine(FlashRed());

        if (currentHealth <= 0)
        {
            if (gamemanager.instance != null)
            {
                gamemanager.instance.updateEnemyCount(-1);
            }

            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        if (spriteRenderer == null)
        {
            yield break;
        }

        Color previousColor = spriteRenderer.color;

        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        spriteRenderer.color = previousColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            isFiring = false;

            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(false);
            }
        }
        
    }

    bool CanTakeDamageFromPlayer()
    {
        if (gamemanager.instance == null || gamemanager.instance.player == null)
        {
            return false;
        }

        Transform player = gamemanager.instance.player.transform;

        float xDistance = player.position.x - transform.position.x;
        float yDistance = Mathf.Abs(player.position.y - transform.position.y);

        //player must be behind enemy
        bool behindEnemy = xDistance > 0f;

        //player cant be too high or too low
        bool validHeight = yDistance <= 1.5f;
        return behindEnemy && validHeight;
    }

    void UpdateGunGlow()
    {
        if (gunGlow == null)
        {
            return;
        }

        float heatPercent = currentHeat / maxHeat;

        gunGlow.color = Color.Lerp(
            coldColor,
            hotColor,
            heatPercent
            );

        gunGlow.enabled = heatPercent > 0.01f;
    }
}
