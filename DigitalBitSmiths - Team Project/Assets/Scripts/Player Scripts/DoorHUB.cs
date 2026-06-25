using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorHUB : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject interactPopup;
    [SerializeField] private GameObject completedMarker;

    //update for score eval 
    [Header("Score Saving")]
    [SerializeField] private bool saveLevelResultBeforeLoad = false;

    private bool playerInRange;

    void Start()
    {
        if (interactPopup != null)
        {
            interactPopup.SetActive(false);
        }

        if (gamemanager.IsLevelComplete(sceneToLoad) && completedMarker != null)
        {
            completedMarker.SetActive(true);
        }
    }

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            LoadDoorScene();
        }
    }

    private void LoadDoorScene()
    {
        if (interactPopup != null)
        {
            //testing for score to save on exit
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (saveLevelResultBeforeLoad && scoreSystem.instance != null)
            {
                scoreSystem.instance.SaveLevelResult(currentSceneName);
                scoreSystem.instance.SubmitFinalScore();

                Debug.Log("DoorHUB saved level result before loading: " + currentSceneName);
            }
            else if (saveLevelResultBeforeLoad && scoreSystem.instance == null)
            {
                Debug.LogWarning("DoorHUB could not save score because scoreSystem.instance is null.");
            }


            interactPopup.SetActive(false);

            // This is stronger than SetActive if the object is accidentally surviving scene loads.
            Destroy(interactPopup);
        }

        SceneManager.LoadScene(sceneToLoad, LoadSceneMode.Single);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            // The "&& interactPopup != null" prevents a crash if it was destroyed
            if (interactPopup != null)
            {
                interactPopup.SetActive(true);


                if(sceneToLoad == "HUB Level ALPHA")
                {
                    //GIVE THE PLAYER ONE UPGRADE PER WIN
                    UpgradeSystem.instance.upgradeAvailable = true;
                    UpgradeSystem.instance.upgradeUsed = false;
                }
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            // Safe check here as well
            if (interactPopup != null)
            {
                interactPopup.SetActive(false);
            }
        }
    }
}