using UnityEngine;

public class UpgradeConsole : MonoBehaviour
{
    public GameObject upgradeUI;        // Your upgrade panel
    public GameObject interactPrompt;   // "Press E to Upgrade" UI

    private bool playerInRange = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        // Only show prompt if upgrade is available
        if (UpgradeSystem.instance.upgradeAvailable && !UpgradeSystem.instance.upgradeUsed)
        {
            playerInRange = true;
            interactPrompt.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerInRange = false;
        interactPrompt.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange)
            return;
       
            if (Input.GetKeyDown(KeyCode.E))
            {
                upgradeUI.SetActive(true);
                interactPrompt.SetActive(false);
                LevelUpUI.instance.ShowUpgradeChoices();
                playerInRange = false;

                // Lock the console for this level
                UpgradeSystem.instance.upgradeUsed = true;
            }
        
    }
}
