using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonfunctions : MonoBehaviour
{

    [SerializeField]  GameObject mainMenuPanel;
    [SerializeField] GameObject settingsPanel;
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        //adding an update to also reset the heart

        //setting a score reset
        if (scoreSystem.instance != null)
        {   
            //can restart current level score 
            scoreSystem.instance.ResetToLevelStartScore();
        }

        // Reset RD Heart collection for this level only
        ResetRDPartsForCurrentLevel();

        
        if (gamemanager.instance != null)
        {
            gamemanager.instance.stateUnpause();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }



    // creating a seperate quit for the in-game menu that doesnt use the primary QUIT function
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        if (gamemanager.instance != null)
        {
            gamemanager.instance.stateUnpause();
        }

        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }


    // Settings (toggle a panel or load a scene? not sure yet)
    public void OpenSettings()
    {
        if (settingsPanel != null)
           settingsPanel.SetActive(true);

        // close main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        // open main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }


    public void quit()
    {   //reset score if the player quits game 
        if (scoreSystem.instance != null)
        {
            scoreSystem.instance.ResetScore();
        }

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif
    }


    // Load Scence - using Menu button "Return to HUB"
    public void LoadScene(string sceneToLoad)
    {

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    //adding a button to lose menu for player respawn
    public void playerRespawn()
    { 
        // adding point deduction on clicking respawn
         if (scoreSystem.instance != null)
    {
        scoreSystem.instance.SubtractScore(50);
    }

        //updated to reset health
        gamemanager.instance.RespawnPlayerFullHealth();
      
    }

    //adding a reset for the rd part collection
    private void ResetRDPartsForCurrentLevel()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Assembly Line Level 1 ALPHA")
        {
            PlayerPrefs.DeleteKey("RDHeart1");
        }
        else if (sceneName == "Home&Garden Level 2 ALPHA")
        {
            PlayerPrefs.DeleteKey("RDHeart2");
        }
        else if (sceneName == "Dyfunct Dystopia Level 3 BETA")
        {
            PlayerPrefs.DeleteKey("RDHeart3");
        }

        PlayerPrefs.Save();

        Debug.Log("RD parts reset for scene: " + sceneName);
    }

    
}
