using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float parallaxStrength = 0.2f;

    private float startX, startY, startZ, cameraStartX, cameraStartY;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;
        startX = transform.position.x;
        startY = transform.position.y;
        startZ = transform.position.z;
        cameraStartX = cameraTransform.position.x;
        cameraStartY = cameraTransform.position.y;
    }

    private void LateUpdate()
    {
        float dx = cameraTransform.position.x - cameraStartX;
        float dy = cameraTransform.position.y - cameraStartY; // track camera Y fully
        transform.position = new Vector3(startX + dx * parallaxStrength, startY, startZ);

    }
}