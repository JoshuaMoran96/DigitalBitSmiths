using UnityEngine;

public class PickupIdleAnimation : MonoBehaviour
{
    [SerializeField] float bobHeight = 0.25f;
    [SerializeField] float bobSpeed = 2f;
    [SerializeField] float rotateSpeed = 30f;

    Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
        float yRotation = Mathf.Sin(Time.time * bobSpeed) * 20f;
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }
}