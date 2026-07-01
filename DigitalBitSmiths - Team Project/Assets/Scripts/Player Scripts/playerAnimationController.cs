using UnityEngine;

public class playerAnimationController : MonoBehaviour
{
    //basic created animations run, idle, jump
    //MUST MAKESURE PARAMETERS and NAMES match with player controller, turned off sprite render
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private playerController playerController;

    [Header("Visual")]
    [SerializeField] private Transform visualRoot;
    [SerializeField] private bool facesLeftByDefault = true;

    private int speedHash;
    private int groundedHash;

    private void Awake()
    {
        speedHash = Animator.StringToHash("Speed");

        // Match Animator parameter name exactly.
        groundedHash = Animator.StringToHash("isGrounded");

        if (playerController == null)
        {
            playerController = GetComponent<playerController>();
        }

        if (visualRoot != null)
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

        if (animator == null)
        {
            Debug.LogError("PlayerAnimationController: No Animator found.");
            enabled = false;
            return;
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerAnimationController: No playerController found.");
            enabled = false;
            return;
        }

        Debug.Log("PlayerAnimationController using Animator on: " + animator.gameObject.name);
        Debug.Log("Animator Controller: " + animator.runtimeAnimatorController.name);
    }

    private void Update()
    {
        //mimcing the sprite renderer flip of player controller
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        animator.SetFloat(speedHash, Mathf.Abs(horizontalInput));
        animator.SetBool(groundedHash, playerController.isGrounded);

        UpdateFacingByMouse();
    }


    private void UpdateFacingByMouse()
    {
        if (visualRoot == null || playerController == null)
        {
            return;
        }

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool mouseIsRight = mouseWorldPos.x > transform.position.x;

        playerController.isFacingRight = mouseIsRight;

        float xScale = Mathf.Abs(visualRoot.localScale.x);

        // AJ's art naturally faces left, so facing right needs negative X scale.
        if (facesLeftByDefault)
        {
            visualRoot.localScale = new Vector3(
                mouseIsRight ? -xScale : xScale,
                visualRoot.localScale.y,
                visualRoot.localScale.z
            );
        }
        else
        {
            visualRoot.localScale = new Vector3(
                mouseIsRight ? xScale : -xScale,
                visualRoot.localScale.y,
                visualRoot.localScale.z
            );
        }
    }

    private void FlipVisual(float horizontalInput)
    {
        if (horizontalInput == 0 || visualRoot == null)
        {
            return;
        }

        float direction = horizontalInput > 0 ? 1f : -1f;

        if (facesLeftByDefault)
        {
            direction *= -1f;
        }

        visualRoot.localScale = new Vector3(
            Mathf.Abs(visualRoot.localScale.x) * direction,
            visualRoot.localScale.y,
            visualRoot.localScale.z
        );
    }
}
