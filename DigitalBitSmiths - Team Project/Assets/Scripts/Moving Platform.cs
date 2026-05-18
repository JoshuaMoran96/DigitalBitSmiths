using UnityEngine;
using System.Collections.Generic;

public class MovingPlatform : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;
    private int i;
    private int movementDirection = 1; // 1 means moving forward, -1 means moving backward

    private Vector2 previousPosition;
    private Vector2 platformVelocity;
    private List<Rigidbody2D> passengers = new List<Rigidbody2D>();

    void Start()
    {
        transform.position = points[startingPoint].position;
        previousPosition = transform.position;
        i = startingPoint; // Starts track movement from the correct starting index
    }

    // Changed to FixedUpdate 
    void FixedUpdate()
    {
        //Setup should be platform moves down track till end of index, then its should reverse course 
        // Check if the platform reached the current target point
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            // Advance to the next point based on direction
            i += movementDirection;

            // If we hit the end of the track, reverse and step back
            if (i >= points.Length)
            {
                movementDirection = -1;
                i = points.Length - 2; // Set target to the second-to-last point
            }
            // If we hit the start of the track, reverse and step forward
            else if (i < 0)
            {
                movementDirection = 1;
                i = 1; // Set target to the second point
            }
        }

        // Move the platform
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.fixedDeltaTime);

        // Calculate actual velocity of the platform this frame
        platformVelocity = ((Vector2)transform.position - previousPosition) / Time.fixedDeltaTime;
        previousPosition = transform.position;

        // Move passengers along with the platform smoothly without locking their controls 
        foreach (Rigidbody2D passenger in passengers)
        {
            if (passenger != null)
            {
                passenger.position += platformVelocity * Time.fixedDeltaTime;
            }
        }
    }

    // Track when objects land on the platform
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
        if (rb != null && !passengers.Contains(rb))
        {
            passengers.Add(rb);
        }
    }

    // Track when objects leave the platform
    private void OnCollisionExit2D(Collision2D collision)
    {
        Rigidbody2D rb = collision.collider.GetComponent<Rigidbody2D>();
        if (rb != null && passengers.Contains(rb))
        {
            passengers.Remove(rb);
        }
    }
}

