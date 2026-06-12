using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    [SerializeField] GameObject brick;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float destroyTimer;
    
    float triggerTime;

    Vector3 originalPos;
    Color originalColor;

    void Start()
    {
        originalPos = transform.position;
        originalColor = sr.color;

        gamemanager.instance.AddPlatforms(this);
    }

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

        brick.SetActive(false);
    }

    public void ResetPlatform()
    {
        //adding a edit so upon checkpoint reset it does not confuse destruction with creation
        //in the event player triggers a checkpoint while platform is timing out
        StopAllCoroutines();
        triggerTime = 0f;

        transform.position = originalPos;
        sr.color = originalColor;
        brick.SetActive(true);
    }
}
