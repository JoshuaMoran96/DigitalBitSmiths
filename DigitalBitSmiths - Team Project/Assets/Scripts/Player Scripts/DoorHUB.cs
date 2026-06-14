using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorHUB : MonoBehaviour
{
    
    [SerializeField] private string sceneToLoad; // which scene to load
    [SerializeField] private GameObject interactPopup; // press 'E' to enter ui
    [SerializeField] private GameObject completedMarker; // marker for completion visual
    [SerializeField] private Vector3 popupOffset = new Vector3(0f, 2f, -1f); // offset of the popup to the door

    bool playerInRange;

    void Start()
    {

        if (interactPopup != null) {
            //interactPopup.transform.position = transform.position + popupOffset;
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
            // 1. Hide the popup immediately so it doesn't leak into the next scene
            if (interactPopup != null)
            {
                interactPopup.SetActive(false);
            }

            // 2. Load the scene
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    //void Update()
    //{
    //    if (playerInRange && Input.GetKeyDown(KeyCode.E))
    //    {
    //        SceneManager.LoadScene(sceneToLoad);
    //    }
    //}
    private void OnTriggerEnter2D(Collider2D other) {

        if (other.CompareTag("Player")) {
            playerInRange = true;

            if (interactPopup != null) {
                interactPopup.transform.position = other.transform.position;
                interactPopup.SetActive(true);
            }
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            if (interactPopup != null)
            {
                interactPopup.SetActive(false);
            }
        }
    }

} 
