using UnityEngine;

public class LoadoutMenuUI : MonoBehaviour
{
    [SerializeField] GameObject loadoutPanel;
    [SerializeField] playerShoot playerShootScript;


    private void Start()
    {
        OpenLoadoutMenu();
        //editing for player shoot bug during selection
        if (playerShootScript == null)
        {
            playerShootScript = FindAnyObjectByType<playerShoot>();
        }
    }

    public void OpenLoadoutMenu()
    {//editing to prevent player shoot during selection
        if (gamemanager.instance != null)
        {
            gamemanager.instance.isPaused = true;
        }
        loadoutPanel.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
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
        //ensuring menu instance is checked by gm
        if (gamemanager.instance != null)
        {
            gamemanager.instance.isPaused = false;
        }
        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
