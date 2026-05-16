using UnityEngine;


public class Respawn : MonoBehaviour
{

    //this is the script for intial respawn and needs to be paired with respawntrigger objects or zones
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gamemanager.instance.RespawnPlayer();
        }
    }
}
