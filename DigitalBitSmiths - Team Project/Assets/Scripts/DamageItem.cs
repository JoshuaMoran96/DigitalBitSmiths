using System;
using UnityEngine;

public class DamageItem : MonoBehaviour, ItemPickup
{
    public float damageMultiplier = 3.0f; // triple damage!
    public float duration = 7.0f;

    // Pass the multiplier up
    public static event Action<float, float> OnDamageCollect;

    public void Collect()
    {
        OnDamageCollect?.Invoke(damageMultiplier, duration);
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
