using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class playerShoot : MonoBehaviour
{
    public float fireRate = 0.2f;
    public Transform firingPoint;
    public GameObject bulletPrefab;
    //public Rigidbody2D bulletRB;

    [SerializeField] float altFireCD;
    [SerializeField] bool canAltFire;

    float timeUntilFire;
   // playerController pm;

   // private void Start()
   // {
       // pm = GetComponent<playerController>();
   // }

    private void Update()
    {   if(!gamemanager.instance.isPaused)
        {
            if (Input.GetMouseButtonDown(0) && timeUntilFire < Time.time)
            {
                Shoot();
                timeUntilFire = Time.time + fireRate;
            }

            if(Input.GetMouseButtonDown(1) && canAltFire)
            {
                AltFire();
            }
        }
    }

    //updating to have players firing be directed and aimed by mouse 
    void Shoot()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = 0f;

        Vector2 shootDirection = (mousePosition - firingPoint.position).normalized; // normalized v not normalized
        //Debug.Log("Shoot direction: " + shootDirection);
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        Instantiate(
            bulletPrefab,
            firingPoint.position,
            Quaternion.Euler(0f, 0f, angle));
        // there wasnt an rb on the after creating it
        //bulletRB = bulletPrefab.GetComponent<Rigidbody2D>();

        //bulletRB.linearVelocity = shootDirection * fireRate;
    }

    void AltFire()
    { 
        StartCoroutine(burstFire());
    }

    IEnumerator burstFire()
    {
        if (canAltFire)
        {
            canAltFire = false;

            Shoot();
            yield return new WaitForSeconds(0.1f);
            Shoot(); 
            yield return new WaitForSeconds(0.1f);
            Shoot();
            yield return new WaitForSeconds(altFireCD);


            canAltFire = true;
        }
      
    }
}
