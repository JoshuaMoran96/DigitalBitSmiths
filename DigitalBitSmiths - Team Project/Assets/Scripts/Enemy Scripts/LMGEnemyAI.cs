using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class LMGEnemyAI : MonoBehaviour, IDamage
{
    //Peacemaker enemy

    [Header("References")]
    [SerializeField] Transform firePoint;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] EnemyHealthBar healthBar;

    [Header("Health")]
    [Range(1, 100)][SerializeField] float maxHealth = 40f;
    [SerializeField] float currentHealth;

    [Header("Shooting")]
    [Range(0.1f, 10)][SerializeField] float fireRate = 0.1f;
    [Range(1, 20)][SerializeField] float bulletSpread = 8f;
    [Range(1, 10)][SerializeField] float burstDuration = 2.5f;
    [Range(1, 10)][SerializeField] float overheatCooldown = 2f;

    [Header("Damage Feedback")]
    [Range(0.1f, 10)][SerializeField] float flashTime = 0.1f;

    [Header("Armor Hit Sound")]
    [SerializeField] AudioSource armorAudioSource;
    [SerializeField] AudioClip armorHitSound;
    [SerializeField] float armorSoundCooldown = 0.15f;

    float nextArmorSoundTime;

    [Header("Weak Spot")]
    [SerializeField] Transform weakSpotCheck;
    [Range(0.1f, 10)][SerializeField] float weakSpotAllowedDistance = 1.5f;

    [Header("Heat")]
    [SerializeField] SpriteRenderer gunGlow;
    [SerializeField] Color coldColor = new Color(1f, 0f, 0f, 0f);
    [SerializeField] Color hotColor = new Color(1f, 0f, 0f, 0.8f);
    [SerializeField] float currentHeat;
    [Range(1, 1000)][SerializeField] float maxHeat = 100f;
    [Range(1, 10)][SerializeField] float heatPerShot = 5f;
    [Range(1, 100)][SerializeField] float coolRate = 25f;

    [Header("Score points")]
    [SerializeField] int scoreValue = 100;
    bool scoreGiven;

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

    //function to play a sound effect of a armor spot 
    void playArmorHitSound()
    {
        if (armorAudioSource == null || armorHitSound == null)
        {
            return;
        }

        if (Time.time < nextArmorSoundTime)
        {
            return;
        }

        armorAudioSource.PlayOneShot(armorHitSound);
        nextArmorSoundTime = Time.time + armorSoundCooldown;
    }

    public void takeDamage(float amount)
    {
        //adding modifier for player score value
        if (!scoreGiven)
        {
            scoreGiven = true;

            if (scoreSystem.instance != null)
            {
                scoreSystem.instance.AddScore(scoreValue);
            }
        }

        if (!CanTakeDamageFromPlayer())
        {  // adding armor sound effect to help show enemy is protected
            playArmorHitSound();
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
