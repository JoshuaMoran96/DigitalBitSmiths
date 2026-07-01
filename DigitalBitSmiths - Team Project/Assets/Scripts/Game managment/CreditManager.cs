using UnityEngine.SceneManagement;
using UnityEngine;


public class CreditManager : MonoBehaviour
{
    // set to display and then return to mainmenu
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            BackToMainMenu();
        }
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
