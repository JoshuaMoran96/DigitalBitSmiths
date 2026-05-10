using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class cameraController : MonoBehaviour
{

    [SerializeField] Transform player;
    [SerializeField] Transform cam;
    [SerializeField] Transform backgroundImage;

    [SerializeField] Rigidbody2D rb;

    float offsetX;
    float offsetY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //cam = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        backgroundFollowCamera();
    }


    void backgroundFollowCamera()
    {
        backgroundImage.position = new Vector3(rb.position.x,0,0);
    }
}
