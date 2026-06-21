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
             mainMenuPanel.SetActive(false);  // hide menu in gameplay scenes
             backgroundImage.SetActive(false);  // hide menu in gameplay scenes
             playerHUD.SetActive(true);
             hoverAudioSource.Stop();
             clickAudioSource.Stop();
             BGMAudio.Stop();
             particles.Stop();
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
        SceneManager.LoadScene(newGameScene);
    }

    // Upgrades
    public void OpenUpgrades()
    {
         if (upgradesPanel != null)
            upgradesPanel.SetActive(true);
        // close main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    
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
    public void OpenEvaluationReport()
    {
         if (EvaluationPanel != null)
            EvaluationPanel.SetActive(true);
        // open main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
    }
    public void CloseEvaluationReport()
    {
        if (EvaluationPanel != null)
            EvaluationPanel.SetActive(false);
        // open main menu 
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(true);
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