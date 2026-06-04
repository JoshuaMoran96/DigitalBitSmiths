using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ladderMovement : MonoBehaviour
{
    //values for ladder , direction, and wheter player is climbing 
    private float vertical;
    private float speed = 8f;
    private bool isLadder;
    private bool isClimbing;
    private float originalGravity;

    [SerializeField] private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //get the set gravity 
    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        originalGravity = rb.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        vertical = Input.GetAxis("Vertical");

        if (isLadder)
        {
            isClimbing = Mathf.Abs(vertical) > 0f;
        }
    }
    private void FixedUpdate()
    {
       if (isClimbing)
        {
            rb.gravityScale = 0f;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, vertical * speed);
        }
       else
        {
            rb.gravityScale = originalGravity;
        }
    }

    //enter the ladder 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Ladder"))
        {
            isLadder = true;
        }
    }
    //exit the ladder
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isLadder = false;
            isClimbing = false;
            rb.gravityScale = originalGravity;
        }
    }
}
