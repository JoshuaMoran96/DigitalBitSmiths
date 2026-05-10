using UnityEngine;
using UnityEngine.UIElements;

public class cameraController : MonoBehaviour
{

    [SerializeField] Transform player;
    gamemanager gm;
    Vector3 cam;
    [SerializeField] Texture2D backgroundImage;

    [SerializeField] Rigidbody2D rb;

    float offsetX;
    float offsetY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //cam = gm.cam.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        backgroundDisplay();
    }


    void backgroundDisplay()
    {

    }
}
