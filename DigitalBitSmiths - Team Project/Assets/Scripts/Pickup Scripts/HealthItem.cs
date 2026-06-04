using System;
using UnityEngine;

public class HealthItem : MonoBehaviour , ItemPickup
{
    public int healAmount = 25;
    //setup a pickup event
    public static event Action<int> OnHealthCollect;

  

    // set a destroy on pickup so it removes the old visual
    public void Collect()
    {
        OnHealthCollect.Invoke(healAmount);

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object touching the item has the "Player" tag
        if (collision.CompareTag("Player"))
        {
            Collect(); // This triggers the event and destroys the item
        }
    }
}
