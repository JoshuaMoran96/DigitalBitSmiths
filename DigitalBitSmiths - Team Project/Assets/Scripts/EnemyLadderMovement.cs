using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLadderMovement : MonoBehaviour
{
    private float speed = 8f;
    private bool isLadder;
    private bool isClimbing;
    private float originalGravity;

    // This replaces Input.GetAxis. 1 = Up, -1 = Down, 0 = Stop
    private float aiVerticalDirection = 0f;

    [SerializeField] private Rigidbody2D rb;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        originalGravity = rb.gravityScale;
    }

    // FUNCTIONS  TO CALL

    // Call this to make the enemy start climbing up or down
    public void StartClimbing(float direction)
    {
        if (isLadder)
        {
            isClimbing = true;
            aiVerticalDirection = Mathf.Clamp(direction, -1f, 1f);
        }
    }

    // Call this to make the enemy stop moving but stay on the ladder
    public void StopMoving()
    {
        aiVerticalDirection = 0f;
    }

    // Call this to force the enemy off the ladder completely
    public void DetachFromLadder()
    {
        isClimbing = false;
        aiVerticalDirection = 0f;
        rb.gravityScale = originalGravity;
    }

    public bool IsOnLadder()
    {
        return isLadder;
    }

    private void FixedUpdate()
    {
        if (isClimbing)
        {
            rb.gravityScale = 0f;
            // Uses Superior Joes  AI's directed movement not player inputs
            rb.linearVelocity = new Vector2(rb.linearVelocityX, aiVerticalDirection * speed);
        }
        else
        {
            rb.gravityScale = originalGravity;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            DetachFromLadder();
        }
    }
}
