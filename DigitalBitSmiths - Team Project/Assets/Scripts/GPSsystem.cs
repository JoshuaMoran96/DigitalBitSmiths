using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GPSsystem : MonoBehaviour
{
    public Transform player;
    public GameObject RDpart;
    public GameObject exitDoor;

    public RectTransform arrowPivot;   // UI pivot
    public Camera cam;                 // Main camera

    public string currentScene;

    void Start()
    {
        player = gamemanager.instance.GetPlayerTransform();
        cam = Camera.main;

        try { RDpart = GameObject.FindWithTag("RD part"); }
        catch { RDpart = null; }

        try { exitDoor = GameObject.Find("Door"); }
        catch { exitDoor = null; }

        if (RDpart == null && exitDoor == null)
            gameObject.SetActive(false);

        currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "BossFight" || currentScene == "HUB Level ALPHA")
        {
            gameObject.SetActive(false);
            return;
        }
    }

    void Update()
    {
        Vector3 targetPos;

        if (RDpart != null && RDpart.activeInHierarchy)
            targetPos = RDpart.transform.position;
        else
            targetPos = exitDoor.transform.position;

        RotateUIArrow(targetPos);
    }

    void RotateUIArrow(Vector3 targetWorldPos)
    {
        Vector3 screenTarget = cam.WorldToScreenPoint(targetWorldPos);
        Vector3 screenPlayer = cam.WorldToScreenPoint(player.position);

        Vector2 dir = screenTarget - screenPlayer;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        arrowPivot.rotation = Quaternion.Euler(0, 0, angle - 90);
    }
}
