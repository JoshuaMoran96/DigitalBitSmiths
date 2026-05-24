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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
    }

    public void quit()
    {
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
}
