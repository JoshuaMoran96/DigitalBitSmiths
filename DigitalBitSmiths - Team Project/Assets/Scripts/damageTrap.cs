using UnityEngine;
using System.Collections;
using Unity.VisualScripting;


public class damageTrap : MonoBehaviour
{
    [SerializeField] SpriteRenderer sMine;
    [SerializeField] GameObject trapType;
    [SerializeField] explosion explosionEffect;
    [SerializeField] CircleCollider2D circleCollider;

    [SerializeField] float damageAmount = 10.0f;
    [SerializeField] float damageRate = 1.0f; // Seconds between hits
    [SerializeField] float flashTime = 0.1f;

    [SerializeField] float riseTargetY = 0.7f;   // mine rise position
    [SerializeField] float riseSpeed = 5f;       //how fast it rises

    float startY;
    bool isDamaging;
    bool isTriggered = false;    
    bool hasExploded = false;  

    Color originalColor;
    private void Start()
    {
        if (trapType == null) trapType = gameObject;

        if (sMine == null)
            sMine = GetComponent<SpriteRenderer>();

        trapType = GameObject.Find(gameObject.name);

        sMine.color = originalColor;
        startY = trapType.transform.position.y;
    }

    private void FixedUpdate()
    {
        if (trapType.name != "Mine" || !isTriggered || hasExploded)
            return;

        // rise to target
        float newY = Mathf.Lerp(trapType.transform.position.y, riseTargetY, Time.deltaTime * riseSpeed);
        trapType.transform.position = new Vector2(trapType.transform.position.x, newY);

        // check how close to target
        if (Mathf.Abs(trapType.transform.position.y - riseTargetY) < 0.01f)
        {
            hasExploded = true;
            BlastRadius();
        }

    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        //fix for spike traps working on enemies
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // 1. Only interact with the Player (or objects with IDamage)
        IDamage dmg = other.GetComponent<IDamage>();

        // 2. Check if we can damage and aren't on cooldown
        if (dmg != null && !isDamaging)
        {
            StartCoroutine(ApplyDamage(dmg));
        }
        if (trapType.name == "Mine" && !isTriggered)
        {
            isTriggered = true;
            StartCoroutine(FlashRed());
        }
    }
    IEnumerator ApplyDamage(IDamage target)
    {
        isDamaging = true;
        target.takeDamage(damageAmount);

        // 3. Wait for the specified rate before allowing damage again
        yield return new WaitForSeconds(damageRate);

        isDamaging = false;
    }

    // Flash for mine
    IEnumerator FlashRed()
    {
        sMine.color = Color.red;
        float newFlashTime = Mathf.Clamp(flashTime, 0.1f, 0.3f);
        yield return new WaitForSeconds(newFlashTime);

        sMine.color = originalColor;
    }
    void BlastRadius() {
        circleCollider.radius = 2.0f;
        if (explosionEffect != null)
            Instantiate(explosionEffect, trapType.transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
