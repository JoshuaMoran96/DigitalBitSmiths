using UnityEngine;

public class playerShoot : MonoBehaviour
{
    public float fireRate = 0.2f;
    public Transform firingPoint;
    public GameObject bulletPrefab;

    float timeUntilFire;
    playerController pm;

    private void Start()
    {
        pm = GetComponent<playerController>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && timeUntilFire < Time.time)
        {
            Shoot();
            timeUntilFire = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        float angle = pm.isFacingRight ? 0f : 180f;
        Instantiate(bulletPrefab, firingPoint.position, Quaternion.Euler(0f, 0f, angle));
    }

}
