using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonfunctions : MonoBehaviour
{
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

    //Not ready yet but when we have a newgame option this schould account for it and score logic
    //public void newGame()
    //{
    //    if (scoreSystem.instance != null)
    //    {
    //        scoreSystem.instance.ResetScore();
    //    }

    //    SceneManager.LoadScene("Level1");
    //}
}
