using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class playerController : MonoBehaviour, IDamage
{
    // UI Stuff
    [SerializeField] Image healthImage;


    /* Just organized the code a bit */
    [SerializeField] Rigidbody2D rb;
    //[SerializeField] Transform cam;

    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintSpeed;
    [SerializeField] int jumpHeight;
    [SerializeField] float groundCheckRadius = 0.2f;

    [SerializeField] int jumpCount; 
    
    public LayerMask groundLayer;
    public Transform groundCheck;

    [SerializeField] bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isGrounded = true;
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // health
        healthImage.fillAmount = HP / 100.0f;

        movement();
        sprint();
       
    }


    // movement 
    void movement() {

        // player directional movement
        float moveInput = Input.GetAxisRaw("Horizontal"); // get the horizontal input 
        rb.linearVelocity = new Vector2(moveInput * speed, rb.linearVelocity.y); // set the x vel to the input * speed and keep the y vel the same

        jump();
    }
    // jump
    void jump() {
        jumpCount = 2;
        isGrounded = false;

        // player ground? and jump input pressed? 
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            jumpCount--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            isGrounded = true;
            Debug.Log("single " + jumpCount);

        } else if (!isGrounded && Input.GetButtonDown("Jump") && jumpCount >= 1) {
            jumpCount--;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
            isGrounded = false;
            Debug.Log("double" + jumpCount);
        }
        else {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y);
        }
        jumpCount = 2;
    }
    // Sprint 
    public void sprint() // not sure if you need public here? 
    {
        if (Input.GetButtonDown("Sprint"))
            speed *= sprintSpeed;
        else if (Input.GetButtonUp("Sprint"))
            speed /= sprintSpeed;
    }


    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundLayer);
    }

    public void takeDamage(int amount) { 
        HP -= amount;

        if (HP <= 0) {
            Debug.Log("LOSE");
            gamemanager.instance.youLose();
        }
    }

    void IDamage.takeDamage(int amount)
    {
        takeDamage(amount);
    }

}

