using UnityEngine;

public class LoadoutMenuUI : MonoBehaviour
{
    [SerializeField] GameObject loadoutPanel;
    [SerializeField] playerShoot playerShootScript;
    gamemanager instance;


    private void Start()
    {
        OpenLoadoutMenu();

        playerShootScript = FindAnyObjectByType<playerShoot>();
    }

    public void OpenLoadoutMenu()
    {
        loadoutPanel.SetActive(true);
        Time.timeScale = 0f;
    }
    public void CloseLoadoutMenu()
    {
        Debug.Log("Confirm button pressed");

        if (playerShootScript != null)
        {
            playerShootScript.RefreshWeapons();
        }
        else
        {
            Debug.LogWarning("playerShootScript is not assigned.");
        }

        loadoutPanel.SetActive(false);
        Time.timeScale = 1f;
    }
}
