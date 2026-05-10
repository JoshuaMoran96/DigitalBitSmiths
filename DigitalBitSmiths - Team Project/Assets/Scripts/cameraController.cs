using UnityEngine;

public class cameraController : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] GameObject backgroundImage;

    [SerializeField] Rigidbody2D rb;
    [SerializeField] Transform cam; 

    float offsetX;
    float offsetY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        backgroundDisplay();   
    }


    void backgroundDisplay() {


        cam.transform.position = rb.transform.position;

    }



}
