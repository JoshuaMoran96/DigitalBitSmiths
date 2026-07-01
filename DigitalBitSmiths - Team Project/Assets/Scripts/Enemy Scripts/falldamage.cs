using UnityEngine;

public class falldamage : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object that fell has the "Enemy" tag
        if (collision.CompareTag("Enemy"))
        {
            // Tell the GameManager to handle any backend logic (scores, waves, etc.)
            if (gamemanager.instance != null)
            {
                gamemanager.instance.OnEnemyFell(collision.gameObject);
                //update enemy tracker to reflect dead enemy
                gamemanager.instance.updateEnemyCount(-1);
            }
            //Destroy the enemy game object instantly
            Destroy(collision.gameObject);
        }
    }
}
