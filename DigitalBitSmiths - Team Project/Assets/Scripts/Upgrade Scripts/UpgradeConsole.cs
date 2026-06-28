using UnityEngine;
using TMPro;

public class UpgradeConsole : MonoBehaviour
{
    public GameObject upgradeUI;        // Your upgrade panel
    public GameObject interactPrompt;   // "Press E to Upgrade" UI

    public SpriteRenderer sr;

    public Sprite ready;
    public Sprite notReady;

    private bool playerInRange = false;

    private void Start()
    {
        if (interactPrompt != null)
            interactPrompt.SetActive(false);

        sr = GetComponent<SpriteRenderer>();
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
        if (interactPrompt.active)
            interactPrompt.SetActive(false);
    }

    private void Update()
    {

        if (!UpgradeSystem.instance.upgradeAvailable)
            sr.sprite = notReady;
        else if (UpgradeSystem.instance.upgradeAvailable)
            sr.sprite = ready;

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

