using UnityEngine;

public class superiorJoeAnimations : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform target;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;

    [Header("Visual")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private bool facesLeftByDefault = true;

    private int speedHash;
    private int groundedHash;

    private void Awake()
    {
        speedHash = Animator.StringToHash("Speed");
        groundedHash = Animator.StringToHash("isGrounded");

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (visualRoot != null && animator == null)
        {
            animator = visualRoot.GetComponent<Animator>();
        }

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (visualRoot == null && animator != null)
        {
            visualRoot = animator.transform;
        }

        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        if (animator == null || rb == null)
        {
            return;
        }

        float speed = Mathf.Abs(rb.linearVelocity.x);

        bool isGrounded = true;

        if (groundCheck != null)
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        animator.SetFloat("Speed", speed);
        animator.SetBool("isGrounded", isGrounded);

        UpdateFacingTowardPlayer();
    }

    private void UpdateFacingTowardPlayer()
    {
        if (visualRoot == null || target == null)
        {
            return;
        }

        bool targetIsRight = target.position.x > transform.position.x;

        float xScale = Mathf.Abs(visualRoot.localScale.x);

        if (facesLeftByDefault)
        {
            visualRoot.localScale = new Vector3(
                targetIsRight ? -xScale : xScale,
                visualRoot.localScale.y,
                visualRoot.localScale.z
            );
        }
        else
        {
            visualRoot.localScale = new Vector3(
                targetIsRight ? xScale : -xScale,
                visualRoot.localScale.y,
                visualRoot.localScale.z
            );
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
