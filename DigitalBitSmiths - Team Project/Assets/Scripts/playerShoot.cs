using UnityEngine;

public class playerShoot : MonoBehaviour
{
    public float fireRate = 0.2f;
    public Transform firingPoint;
    public GameObject bulletPrefab;

    float timeUntilFire;
   // playerController pm;

   // private void Start()
   // {
       // pm = GetComponent<playerController>();
   // }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && timeUntilFire < Time.time)
        {
            Shoot();
            timeUntilFire = Time.time + fireRate;
        }
    }

    //updating to have players firing be directed and aimed by mouse 
    void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 shootDirection = mousePosition - firingPoint.position;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        Instantiate(
            bulletPrefab,
            firingPoint.position,
            Quaternion.Euler(0f, 0f, angle));
    }

}
