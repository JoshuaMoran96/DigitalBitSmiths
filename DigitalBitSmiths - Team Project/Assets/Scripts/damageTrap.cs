using UnityEngine;
using System.Collections;
public class damageTrap : MonoBehaviour
{
    [SerializeField] float damageAmount = 10.0f;
    [SerializeField] float damageRate = 1.0f; // Seconds between hits

    bool isDamaging;

    private void OnTriggerStay2D(Collider2D other)
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
    }

    IEnumerator ApplyDamage(IDamage target)
    {
        isDamaging = true;
        target.takeDamage(damageAmount);

        // 3. Wait for the specified rate before allowing damage again
        yield return new WaitForSeconds(damageRate);

        isDamaging = false;
    }

}
