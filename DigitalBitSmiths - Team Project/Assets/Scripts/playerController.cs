using UnityEngine;

public class playerController : MonoBehaviour
{

    [SerializeField] Rigidbody2D rb;


    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintSpeed;
    [SerializeField] int jumpHeight;
    public LayerMask groundLayer;
    public Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;

    float moveX;
    float moveY;

    bool isGrounded;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0) ;
        }


        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2 (moveInput * speed, rb.linearVelocity.y);

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHeight);
        }

        sprint();
    }

    public void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
            speed *= sprintSpeed;
        else if (Input.GetButtonUp("Sprint"))
            speed /= sprintSpeed;
    }
}
