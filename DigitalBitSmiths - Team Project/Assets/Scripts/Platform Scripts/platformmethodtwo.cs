using System.Collections.Generic;
using UnityEngine;

public class platformmethodtwo : MonoBehaviour
{
    public float speed;
    public int startingPoint;
    public Transform[] points;
    private int i;

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
        // Move the platform
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++;
            if (i == points.Length)
            {
                i = 0;
            }
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.fixedDeltaTime);

        //Calculate actual velocity of the platform this frame
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
