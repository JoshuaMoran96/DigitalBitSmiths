using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
using TMPro;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    // UI Stuff
    [SerializeField] Image healthImage;
    /* Just organized the code a bit */
    [SerializeField] Rigidbody2D rb;
    [SerializeField] CapsuleCollider2D capsuleCollider;
    //[SerializeField] Transform cam;

    private SpriteRenderer spriteRenderer;

    [Header("----- Audio -----")]
    [SerializeField] public AudioSource audPlayer;
    [SerializeField] AudioClip[] audHurt;
    [SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audSteps;
    [SerializeField] float audStepsVol;


    bool isPlayingStep;
    bool isSprinting;

    [Header("Health")]
    [SerializeField] float currentHP;
    [SerializeField] float maxHP = 100;
    public float originalHP;


    [Header("Movement")]
    [SerializeField] float speed = 5f;
    //[SerializeField] float sprintSpeed;
    [SerializeField] int jumpHeight;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] SpriteRenderer faceDir;
    [SerializeField] int jumpCount;

    //setting a default for damage boost
    [Header("Damage Stats")]
    [SerializeField] float baseDamage = 10f; // Your default attack power
    public float currentDamage;
    private Coroutine damageBoostCoroutine;

    public bool upgradeTesterOpen = false;

    // Dash Stats
    [Header("Dash Settings")]
    float dashTime;
    public float dashSpeed = 20.0f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.75f;
    public bool canDash;
    public bool isDashing;
    public float iFrameDuration = .25f;

    [Header("Player Effects")]
    [SerializeField] ParticleSystem dust;
    Color origColor;
    
    public LayerMask groundLayer;

    public LayerMask enemyLayer;
    public LayerMask bulletLayer;

    //public Transform groundCheck;

    [SerializeField] float rayLength = 0.5f;

    float moveInput;

    Vector2 movDir;

    [SerializeField] bool isGrounded;

    //variables for a firing mechanic
    public bool isFacingRight;

    //Adding vatiables for player speed boost
    private float normalSpeed = 10.0f;
    private float currentSpeed;
    private Coroutine speedBoostCoroutine;



   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //attempting to set a default
        maxHP = 100f;
        originalHP = maxHP;
        currentHP = maxHP;
      
        //base speed is assigned
        currentSpeed = speed; 
        //base damage is assigned
        currentDamage = baseDamage;



        spriteRenderer = GetComponent<SpriteRenderer>();

        origColor = spriteRenderer.color;

        canDash = true;

        isGrounded = true; // player starting state will be grounded
        currentHP = 100; // start the player off with full hp
        rb = GetComponent<Rigidbody2D>(); // set the rb comp

        //on start assign health image via UI
        if (healthImage == null)
        {
            healthImage = GameObject.Find("Health").GetComponent<Image>();
        }
        
    }

    // Update is called once per frame
    void Update() // read input
    {
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        getInput();
        flipDir(mouseWorldPos.x < rb.transform.position.x);

        updateHealthBar();

        UpgradeTester();

    }

    private void FixedUpdate() // apply physics to rb using that input
    {

        if (isDashing)
            return;
        //update for speedboost
        rb.linearVelocity = new Vector2(moveInput * currentSpeed, rb.linearVelocity.y);
        //rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);  orginal
        bool isMoving = Mathf.Abs(rb.linearVelocity.x) > 0.1f;

        // Player movement
        //this is the area for the dust vfx
        if (isMoving && isGrounded)
        {
            if (!dust.isEmitting) dust.Play();
        }
        else {
            if (dust.isEmitting) dust.Stop();
        }
            groundCheck();

    }

    // flip dir 
    void flipDir(bool flip) {
        // flip sprite  - the sprite render has a bool Flip x,y
        faceDir.flipX = flip;
        isFacingRight = !flip;

    }

    // jump
    void Jump()
    {
        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount--;
            Debug.Log("Grounded 3 " + isGrounded);
        }
        else if (!isGrounded && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount--;
        }

    }

    // get input ()
    void getInput() 
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if( (moveInput > 0 || moveInput < 0) && !isPlayingStep && isGrounded)
        {
            StartCoroutine(playStep());
        }

        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount > 0))
        {
            Jump();
            audPlayer.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);

        }

        if (Input.GetButtonDown("Sprint"))
        {
            dash();
        }

    }

    // check ground
    void groundCheck() {
        Vector2 bottomOfPlayer = new Vector2(transform.position.x, capsuleCollider.bounds.min.y); // get the bottom bounds of the cap coll  

        isGrounded = Physics2D.Raycast(bottomOfPlayer, Vector2.down, rayLength, groundLayer);
        
        if (isGrounded && rb.linearVelocity.y <= 0.5f)
        {
            jumpCount = 2;
        }

    }

    // function for HP color change
    void updateHealthBar() {

        if (healthImage == null)
        {
            return;
        }

        float t = Time.deltaTime;
        float hpAmount = Mathf.Clamp01(currentHP / maxHP); // making sure it doesnt go below 0 or above 1
        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, hpAmount, Time.deltaTime * lerpSpeed);
        //healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, hpAmount, t * lerpSpeed);

        if (hpAmount >= 0.5f)
        {
            healthImage.color = Color.green;

        }
        else if ( hpAmount >= 0.3f)
        {
            healthImage.color = Color.coral;

        }
        else if (hpAmount > 0f)
        {
            healthImage.color = Color.red;
        }
        else {
            healthImage.fillAmount = 0.0f;
        }
            
    }

    // player recieveing damage
    public void takeDamage(float amount) {

        currentHP -= amount;
        Debug.Log("Current HP: " + currentHP);
        StartCoroutine(FlashRed());
        audPlayer.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        if (currentHP <= 0) {
            Debug.Log("LOSE");
            gamemanager.instance.youLose();
        }
    }

    void IDamage.takeDamage(float amount)
    {
        if (isDashing)
        {
            return;
        }
        takeDamage(amount);
    }

    //Dash
    public void dash()
    {
        if (!canDash)
            return;

        canDash = false;
        isDashing = true;

        float dir = rb.linearVelocity.x;

        rb.linearVelocity = new Vector2(dir * dashSpeed, rb.linearVelocity.y);

        dashTime = Time.time + dashDuration;

        StartCoroutine(iFrames());

        Invoke(nameof(endDash), dashDuration);

        Invoke(nameof(resetDash), dashCooldown);

    }

    void endDash()
    {
        isDashing = false;
    }

    void resetDash()
    {
        canDash = true;
    }

    IEnumerator iFrames()
    {
        //int enemyLayer = LayerMask.NameToLayer("Enemy");

        int bulletLayer = LayerMask.NameToLayer("EnemyBullet");

        //Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, true);
        Physics2D.IgnoreLayerCollision(gameObject.layer, bulletLayer, true);

        SetOpacity(0.25f);

        yield return new WaitForSeconds(iFrameDuration);

        SetOpacity(1.0f);

        //Physics2D.IgnoreLayerCollision(gameObject.layer, enemyLayer, false);

        Physics2D.IgnoreLayerCollision(gameObject.layer, bulletLayer, false);
    }

    public void SetOpacity(float alpha)
    {
        Color color = spriteRenderer.color;
        color.a = Mathf.Clamp01(alpha);
        spriteRenderer.color = color;

    }

    IEnumerator FlashRed()
    {
        spriteRenderer.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        spriteRenderer.color = origColor;
    }

    //Adding a heal mechanic for player health , meant to kick in on Health Item pickup
    void Heal(int amount)
    {
        //add the healing effect amount to players currentHP
        currentHP += amount;
        // Prevents health from exceeding max limits
        if (currentHP > maxHP)
        {
            currentHP = maxHP;
        }
    }

    // throwing in a unsubscription to prevent a static pickup error
    //updating to carry out for additional items
    private void OnEnable()
    {
        HealthItem.OnHealthCollect += Heal;
        DamageItem.OnDamageCollect += StartDamageBoost;
        SpeedItem.OnSpeedCollect += StartSpeedBoost;
    }

    private void OnDisable()
    {
        HealthItem.OnHealthCollect -= Heal;
        DamageItem.OnDamageCollect -= StartDamageBoost;
        SpeedItem.OnSpeedCollect -= StartSpeedBoost;
    }


    //Player Damage boost function
    public void StartDamageBoost(float multiplier, float duration)
    {
        if (damageBoostCoroutine != null)
        {
            StopCoroutine(damageBoostCoroutine);
        }
        damageBoostCoroutine = StartCoroutine(DamageBoostRoutine(multiplier, duration));
    }

    private IEnumerator DamageBoostRoutine(float multiplier, float duration)
    {
        currentDamage = baseDamage * multiplier;
        Debug.Log("DAMAGE BOOST ACTIVE! Current Damage: " + currentDamage);

        yield return new WaitForSeconds(duration);

        currentDamage = baseDamage;
        Debug.Log("Damage boost expired.");
        damageBoostCoroutine = null;
    }

    //updating for damage boost to mesh with new weapons
    public float GetDamageMultiplier()
    {
        return currentDamage / baseDamage;
    }
    public float GetCurrentDamage()
    {
        return currentDamage;
    }


    //Speeding up player for speed item boost
    public void StartSpeedBoost(float multiplier, float duration)
    {
        // If the player already has a speed boost, stop it so they don't stack infinitely
        if (speedBoostCoroutine != null)
        {
            StopCoroutine(speedBoostCoroutine);
        }

        speedBoostCoroutine = StartCoroutine(SpeedBoostRoutine(multiplier, duration));
    }

    // Coroutine to handle the speed timer safely
    private IEnumerator SpeedBoostRoutine(float multiplier, float duration)
    {
        currentSpeed = normalSpeed * multiplier;

        yield return new WaitForSeconds(duration);

        currentSpeed = normalSpeed;
        speedBoostCoroutine = null;
    }

    //Setting up a function to help reset players health upon respawn aftera misadventure, similar setup to heal but only works after death
    public void ResetHealth()
    {
        currentHP = maxHP;

        if (healthImage != null)
        {
            healthImage.fillAmount = 1f;
            healthImage.color = Color.green;
        }
    }

    IEnumerator playStep()
    {
        isPlayingStep = true;

        audPlayer.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        yield return new WaitForSeconds(0.2f);

        isPlayingStep = false;
    }

    //setting trigger for pickup of items to add a score value
    //part of score system
    //using a tag system to update score
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "power pickup")
        {
            // refer to totalScore variable  will add 10pts on power up pickup
            scoreSystem.totalScore += 10;
            //display on screen
            if (scoreSystem.instance != null)
            {
                scoreSystem.instance.AddScore(10);
            }
            // Debug.Log(scoreSystem.totalScore);   just displays to dev
            //destroy object already carried out by pickup this is just redundant check to 
            collision.gameObject.SetActive(false);
            
        }
    }


    //
    //
    //
    // UPGRADE SCRIPTS
    //
    //
    //

    public void AddHP(float amount)
    {
        maxHP += originalHP * (1f + amount);
        currentHP += originalHP * (1f + amount);
    }

    public void AddDamage(float amount)
    {
        currentDamage += baseDamage * (1f + amount);
        baseDamage = 10;
    }

    public void AddSpeed(float amount)
    {
        currentSpeed *= (1f + amount);
        
    }

    public void UpgradeTester()
    {
        if(Input.GetKeyDown(KeyCode.U) && !upgradeTesterOpen)
        {
            upgradeTesterOpen = true;
            LevelUpUI.instance.ShowUpgradeChoices();
            
        }
    }

}