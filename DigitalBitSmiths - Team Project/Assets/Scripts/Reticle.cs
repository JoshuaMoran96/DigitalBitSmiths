using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField] Transform firePoint;
    [SerializeField] Transform reticle;

    public Vector2 AimDirection { get; private set; }

    void Update()
    {
       
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        AimDirection = (mouseWorldPos - firePoint.position).normalized;

        reticle.position = mouseWorldPos;
    }
}
