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

    float moveInput;
    Vector2 input;

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
        healthImage.fillAmount = HP / 100f;

        moveInput = Input.GetAxisRaw("Horizontal");

        sprint();

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (Input.GetButtonDown("Jump"))
            jump();

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

