using UnityEngine;
using UnityEngine.SceneManagement;

public class level5DoorLock : MonoBehaviour
{
    [SerializeField] string level5SceneName = "Level5";
    private bool playerInRange;

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryEnterLevel5();
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

    private void TryEnterLevel5()
    {
        bool superiorJoeDefeated = PlayerPrefs.GetInt("SuperiorJoeDefeated", 0) == 1;

        if (superiorJoeDefeated)
        {
            Debug.Log("Entering Level 5");
            SceneManager.LoadScene(level5SceneName);
        }
        else
        {
            Debug.Log("Door Locked! Defeat Superior Joe first.");
        }
    }
}
