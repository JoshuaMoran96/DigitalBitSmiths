using UnityEngine;

public class checkpoint : MonoBehaviour
{
    //this is the script to help update the new checkpoint for respawn and will be handled by GM
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Pass this exact object's position directly
            gamemanager.instance.UpdateRespawnPoint(this.transform);

            GetComponent<Collider2D>().enabled = false;
        }
    }
}
