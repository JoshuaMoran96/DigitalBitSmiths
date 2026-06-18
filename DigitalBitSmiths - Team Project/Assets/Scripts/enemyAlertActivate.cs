using UnityEngine;

public class enemyAlertActivate : MonoBehaviour
{// set the enemy ai script, and attack script in here to disable
    [Header("Enemy scripts to turn off until RD Part is collected")]
    [SerializeField] private MonoBehaviour[] scriptsToDisable;
    // set the two know layers to switch from Lockedenemy is Inactive  Enemy is active
    [Header("Layer Settings")]
    [SerializeField] private string lockedLayerName = "Lockedenemy";
    [SerializeField] private string activeLayerName = "Enemy";


    
    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        LockEnemy();
    }

    public void ActivateEnemy()
    {
        gameObject.layer = LayerMask.NameToLayer(activeLayerName);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }
    }

      

    private void LockEnemy()
    {
        gameObject.layer = LayerMask.NameToLayer(lockedLayerName);

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        //if (damageHitbox != null)
        //{
        //    damageHitbox.enabled = false;
        //}

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}
