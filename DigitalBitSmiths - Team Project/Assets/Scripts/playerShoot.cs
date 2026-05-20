using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class playerShoot : MonoBehaviour
{
    public float fireRate = 0.2f;
    public Transform firingPoint;
    public GameObject bulletPrefab;
    //public Rigidbody2D bulletRB;

    [SerializeField] float altFireCD;
    [SerializeField] int altFireCount;
    [SerializeField] bool canAltFire;
    [SerializeField] Rigidbody2D rb;

    gamemanager instance;

    float timeUntilFire;
    //attempting to update for damage boost
    playerController pm;

    private void Start()
    {
     pm = GetComponent<playerController>();
   }

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
        


        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        //spawn bullet store reference
        GameObject newBullet = Instantiate(bulletPrefab, firingPoint.position, Quaternion.Euler(0f, 0f, angle));

        // Pass the dynamic damage value from playerController into the bullet script
        if (newBullet.TryGetComponent(out bullet bulletComponent))
        {
            // Fallback to base damage (10) if the controller reference is somehow missing
            float damageToGive = (pm != null) ? pm.currentDamage : 10f;
            bulletComponent.bulletDamage = damageToGive;
        }

        //Instantiate(
        //    bulletPrefab,
        //    firingPoint.position,
        //    Quaternion.Euler(0f, 0f, angle));
        // there wasnt an rb on the after creating it
        //bulletRB = bulletPrefab.GetComponent<Rigidbody2D>();

        //bulletRB.linearVelocity = shootDirection * fireRate;
    }

    void AltFire()
    { 
        StartCoroutine(burstFire());
    }

    void ApplyRecoil()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorld - (Vector2)firingPoint.position).normalized;

        Vector2 recoil = -direction * 20f;


        rb.linearVelocity += recoil;
    }

    IEnumerator burstFire()
    {
        if (canAltFire)
        {
            canAltFire = false;

            ApplyRecoil();

            for (int i = 0; i < altFireCount; i++)
            {
                Shoot();
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitForSeconds(altFireCD);


            canAltFire = true;
        }
      
    }
}
