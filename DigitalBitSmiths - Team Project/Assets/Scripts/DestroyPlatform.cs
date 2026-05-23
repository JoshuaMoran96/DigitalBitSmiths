using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    [SerializeField] GameObject brick;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float destroyTimer;
    
    float triggerTime;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerTime = 0.0f;
        if(!collision.CompareTag("Player"))
        {
            return;
        }

        StartCoroutine(DestroyingPlatform());

    }
    

    IEnumerator DestroyingPlatform()
    {
        while(triggerTime < destroyTimer)
        {
            Color c = sr.color;
            c.a -= 0.2f;
            sr.color = c;

            triggerTime += 0.2f;

            yield return new WaitForSeconds(0.2f);
        }

        Destroy(brick);
    }
}
