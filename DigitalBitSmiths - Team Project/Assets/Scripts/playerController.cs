using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    // UI Stuff
    [SerializeField] Image healthImage;

    gamemanager gm;
    /* Just organized the code a bit */
    [SerializeField] Rigidbody2D rb;

    //[SerializeField] Transform cam;
  
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
    [SerializeField] float speed;
    [SerializeField] float sprintSpeed;
    [SerializeField] int jumpHeight;
    [SerializeField] float groundCheckRadius = 0.2f;

    [SerializeField] int jumpCount; 
    
    public LayerMask groundLayer;
    public Transform groundCheck;

    float moveInput;
    Vector2 input;

    [SerializeField] bool isGrounded;

    //variables for a firing mechanic
    public bool isFacingRight = true;

    float nextFireTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGrounded = true; // player starting state will be grounded
        currentHP = 10; // start the player off with full hp
        rb = GetComponent<Rigidbody2D>(); // set the rb comp
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        updateHealthBar();
   
        
        
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && rb.linearVelocity.y <= 0.01f)
        {
            jumpCount = 2;
        }

        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y);
    }

    void movement() { 

        moveInput = Input.GetAxisRaw("Horizontal");
        //adding some shooting facing movement Right it fires left it does not
       /* if (moveInput > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
            isFacingRight = true;
        }
        else if (moveInput < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
            isFacingRight = false;
       }*/ 


        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump"))
            jump();
    }

    // function for HP color change
    void updateHealthBar() {

        float t = Time.deltaTime;
        float hpAmount = Mathf.Clamp01(currentHP / maxHP); // making sure it doesnt go below 0 or above 1
        float hpLerp = Mathf.Lerp(hpAmount, maxHP, t);

        if (hpAmount >= 0.5f)
        {
            healthImage.color = Color.green;
        }
        else if ( hpAmount >= 0.3f)
        {
            healthImage.color = Color.orange;
        }
        else if (hpAmount >= 0.1f)
        {
            healthImage.color = Color.orange;
        }
        else {
            healthImage.fillAmount = 0.0f;
        }
            
    }


    // jump
    void jump() {

        if (isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount--;
            return;
        }

        if (!isGrounded && jumpCount > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            jumpCount--;
        }
    }


    // Sprint 
    public void sprint() // not sure if you need public here? 
    {
        if (Input.GetButtonDown("Sprint"))
            speed *= sprintSpeed;
        else if (Input.GetButtonUp("Sprint"))
            speed /= sprintSpeed;
    }

    public void takeDamage(float amount) {
        currentHP -= amount;
        
        if (currentHP <= 0) {
            Debug.Log("LOSE");
            gamemanager.instance.youLose();
        }
    }

    void IDamage.takeDamage(float amount)
    {

        takeDamage(amount);
    }

}

