using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorHUB : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private GameObject interactPopup;
    [SerializeField] private GameObject completedMarker;

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