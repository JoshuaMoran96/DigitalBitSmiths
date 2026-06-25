using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Scene Names")]
    [SerializeField] string newGameScene = "tutorial Level 0 ALPHA";
    [SerializeField] string quitScene = "MainMenu";

    [Header("Panels")]
    [SerializeField] GameObject mainMenuPanel;
     [SerializeField] GameObject backgroundImage;
    [SerializeField] AudioSource hoverAudioSource;
    [SerializeField] AudioSource clickAudioSource;
    [SerializeField] AudioSource BGMAudio;
    [SerializeField] ParticleSystem particles;
    [SerializeField] GameObject playerHUD;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject upgradesPanel;
    [SerializeField] GameObject EvaluationPanel;


    void Start()
    {
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            // Gameplay scene setup
            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(false);

            if (backgroundImage != null)
                backgroundImage.SetActive(false);

            if (playerHUD != null)
                playerHUD.SetActive(true);

            if (hoverAudioSource != null)
                hoverAudioSource.Stop();

            if (clickAudioSource != null)
                clickAudioSource.Stop();

            if (BGMAudio != null)
                BGMAudio.Stop();

            if (particles != null)
                particles.Stop();
        }
        else
        {
            // Main Menu scene setup
            Time.timeScale = 1f;

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            if (mainMenuPanel != null)
                mainMenuPanel.SetActive(true);

            if (backgroundImage != null)
                backgroundImage.SetActive(true);

            if (playerHUD != null)
                playerHUD.SetActive(false);

            if (settingsPanel != null)
                settingsPanel.SetActive(false);

            if (upgradesPanel != null)
                upgradesPanel.SetActive(false);

            if (EvaluationPanel != null)
                EvaluationPanel.SetActive(false);

            if (BGMAudio != null && !BGMAudio.isPlaying)
                BGMAudio.Play();

            if (particles != null && !particles.isPlaying)
                particles.Play();
        }

    }

    // Continue will load the last saved or played level
    public void Continue()
    {
        // For now it loads the same as New Game.
        // Later it shoould read a saved level name from PlayerPrefs and load that maybe.
        string savedLevel = PlayerPrefs.GetString("LastLevel", newGameScene); // need to take a look
        SceneManager.LoadScene(savedLevel);
    }

    // Start from the first level
    public void NewGame()
    {
        //reset the score value 
        if (scoreSystem.instance != null)
        {
            scoreSystem.instance.ResetScore();
        }
        else
        {
            scoreSystem.totalScore = 0;
            scoreSystem.levelStartScore = 0;
        }
        //reset part count just hard coding
        PlayerPrefs.DeleteKey("RDHeart1");
        PlayerPrefs.DeleteKey("RDHeart2");
        PlayerPrefs.DeleteKey("RDHeart3");

        // Reset completed levels (if you're tracking them)
        PlayerPrefs.DeleteKey("LevelComplete_tutorial Level 0 ALPHA");
        PlayerPrefs.DeleteKey("LevelComplete_Assembly Line Level 1 ALPHA");
        PlayerPrefs.DeleteKey("LevelComplete_Home&Garden Level 2 ALPHA");
        PlayerPrefs.DeleteKey("LevelComplete_Dysfunct Dystopia Level 3 BETA");
        PlayerPrefs.DeleteKey("LevelComplete_Research Level 4 Alpha");
        PlayerPrefs.DeleteKey("SuperiorJoeDefeated");
        PlayerPrefs.Save();

        SceneManager.LoadScene(newGameScene);
    }

    // Upgrades
    public void OpenUpgrades()
    {
         if (SceneManager.GetActiveScene().name != "MainMenu") 
            if (upgradesPanel != null)
                upgradesPanel.SetActive(true);
        // close main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        playerHUD.SetActive(true);
    
    }
    public void CloseUpgrades()
    {
         if (upgradesPanel != null)
            upgradesPanel.SetActive(false);
        // open main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }

    // Evaluation Report
    //enforcing the evaluation report to open, after player exits to main menu
    public void OpenEvaluationReport()
    {
        Time.timeScale = 1f;

        if (EvaluationPanel != null)
        {
            EvaluationPanel.SetActive(true);
            EvaluationPanel.transform.SetAsLastSibling();

            EvalReport reportUI = EvaluationPanel.GetComponentInChildren<EvalReport>(true);

            if (reportUI != null)
            {
                reportUI.RefreshReport();
            }
         }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(false);
        }
    }



    public void CloseEvaluationReport()
    {
        if (EvaluationPanel != null)
        {
            EvaluationPanel.SetActive(false);
        }

        if (mainMenuPanel != null)
        {
            mainMenuPanel.SetActive(true);
        }
    }

    // adding a credits button
    // Credits
    public void OpenCredits()
    {
        SceneManager.LoadScene("credits");
    }
    
   

    // Settings (toggle a panel or load a scene? not sure yet)
    public void OpenSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(true);
        // close main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        // open main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
    }


    // Quit
    public void Quit()
    {
         if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            Debug.Log("Quit pressed");  // won't quit in the editor, only in a build
            SceneManager.LoadScene(quitScene);
        } else
        {
            #if UNITY_EDITOR
              UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif

        }
        

    }

    

}