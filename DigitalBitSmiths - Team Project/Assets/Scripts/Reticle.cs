using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField] Transform firePoint;

    gamemanager instance;

    //[SerializeField] Transform reticle;

    public Vector2 AimDirection { get; private set; }

    private void Start()
    {
        if (firePoint == null)
        {
            firePoint = GameObject.Find("firePoint").GetComponent<Transform>();
        }
    }

    void Update()
    {
       
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0f;

        AimDirection = (mouseWorldPos - firePoint.position).normalized;

        transform.position = mouseWorldPos;
    }
}
