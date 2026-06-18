using UnityEngine;
using UnityEngine.SceneManagement;

public class level4DoorLock : MonoBehaviour
{
    [SerializeField] string level4SceneName = "Research Level 4 BETA";
    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryEnterLevel4();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    private void TryEnterLevel4()
    {
        bool hasHeart1 = PlayerPrefs.GetInt("RDHeart1", 0) == 1;
        bool hasHeart2 = PlayerPrefs.GetInt("RDHeart2", 0) == 1;
        bool hasHeart3 = PlayerPrefs.GetInt("RDHeart3", 0) == 1;

        if (hasHeart1 && hasHeart2 && hasHeart3)
        {
            Debug.Log("Entering Level 4");
            SceneManager.LoadScene(level4SceneName);
        }
        else
        {
            Debug.Log("Door Locked! All RD parts required.");
        }
    }
}
