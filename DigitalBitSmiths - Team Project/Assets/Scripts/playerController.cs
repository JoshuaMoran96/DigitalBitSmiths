using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;
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

    [Header("Health")]
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;


    [Header("Movement")]
    [SerializeField] float speed;
    //[SerializeField] float sprintSpeed;
    [SerializeField] int jumpHeight;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] float lerpSpeed = 5f;
    [SerializeField] SpriteRenderer faceDir; 
    [SerializeField] int jumpCount;


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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

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

        if (gamemanager.instance.menuActive != null) {
            spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate() // apply physics to rb using that input
    {

        if (isDashing)
            return;

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
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

        if (Input.GetButtonDown("Jump") && (isGrounded || jumpCount > 0))
        {
            Jump();

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

        float t = Time.deltaTime;
        float hpAmount = Mathf.Clamp01(currentHP / maxHP); // making sure it doesnt go below 0 or above 1
        healthImage.fillAmount = Mathf.Lerp(healthImage.fillAmount, hpAmount, t * lerpSpeed);

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

    // Sprint
    //public void sprint()
    //{
    //    if (Input.GetButtonDown("Sprint"))
    //        speed *= sprintSpeed;
    //    else if (Input.GetButtonUp("Sprint"))
    //        speed /= sprintSpeed;
    //}

    // damage stuff
    public void takeDamage(float amount) {

        currentHP -= amount;
        Debug.Log("Current HP: " + currentHP);
        StartCoroutine(FlashRed());
        
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
 
}