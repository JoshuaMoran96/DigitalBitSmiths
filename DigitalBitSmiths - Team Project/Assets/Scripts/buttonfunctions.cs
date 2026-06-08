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
        //setting a score reset
        if (scoreSystem.instance != null)
        {//can restart current level score 
            scoreSystem.instance.ResetToLevelStartScore();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        if (gamemanager.instance != null)
        {
            gamemanager.instance.stateUnpause();
        }
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

    public void loadLevel(int lvl)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        gamemanager.instance.stateUnpause();
    }

    //adding a button to lose menu for player respawn
    public void playerRespawn()
    {//updated to reset health
        gamemanager.instance.RespawnPlayerFullHealth();
        gamemanager.instance.stateUnpause();
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
