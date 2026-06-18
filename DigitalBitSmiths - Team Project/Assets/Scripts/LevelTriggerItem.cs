using UnityEngine;

public class LevelTriggerItem : MonoBehaviour, ItemPickup
{
    // Implements the interface method
    public void Collect()
    {
        // Notify the Game Manager to wake up the enemies
        gamemanager gm = Object.FindAnyObjectByType<gamemanager>();
        if (gm != null)
        {
            gm.TriggerEnemyActivation();
        }

        // Play sound effects or particles here if we decide on one tbd

        //Destroy the item
        //may need to add as second tracker for overall quest collection
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the colliding object is the player
        if (collision.CompareTag("Player"))
        {
            Collect();
        }
    }
}
