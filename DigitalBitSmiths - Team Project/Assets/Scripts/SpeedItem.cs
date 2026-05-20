using System;
using UnityEngine;

public class SpeedItem : MonoBehaviour
{
    public float speedMultiplier = 1.5f;
    public float duration = 5.0f;

    // Passes both the boost amount and how long it lasts
    public static event Action<float, float> OnSpeedCollect;

    public void Collect()
    {
        OnSpeedCollect?.Invoke(speedMultiplier, duration);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }
}
