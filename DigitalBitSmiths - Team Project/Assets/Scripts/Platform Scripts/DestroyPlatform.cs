using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class DestroyPlatform : MonoBehaviour
{
    [SerializeField] GameObject brick;
    [SerializeField] SpriteRenderer sr;
    [SerializeField] float destroyTimer;
    [SerializeField] Sprite[] breakFrames;
    [SerializeField] float idleFrameRate = 0.15f;

    float triggerTime;
    Vector3 originalPos;
    Color originalColor;
    Sprite originalSprite;

    Coroutine idleCoroutine;   // FIX 1: cache the reference
    bool isBreaking = false;   // FIX 2: prevent re-triggering

    void Start()
    {
        originalPos = transform.position;
        originalColor = sr.color;
        originalSprite = sr.sprite;

        gamemanager.instance.AddPlatforms(this);
        idleCoroutine = StartCoroutine(IdleAnimation());
    }

    IEnumerator IdleAnimation()
    {
        while (true)
        {
            for (int i = 0; i < 5; i++)
            {
                sr.sprite = breakFrames[i];
                yield return new WaitForSeconds(idleFrameRate);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // FIX 2: ignore if not player OR already breaking
        if (!collision.CompareTag("Player") || isBreaking) return;

        isBreaking = true;
        StartCoroutine(DestroyingPlatform());
    }

    IEnumerator DestroyingPlatform()
    {
        // FIX 1: stop idle using cached reference
        if (idleCoroutine != null)
        {
            StopCoroutine(idleCoroutine);
            idleCoroutine = null;
        }

        float interval = destroyTimer / (breakFrames.Length - 5);

        for (int i = 5; i < breakFrames.Length; i++)
        {
            sr.sprite = breakFrames[i];
            triggerTime += interval;
            yield return new WaitForSeconds(interval);
        }

        brick.SetActive(false);
    }

    public void ResetPlatform()
    {
        StopAllCoroutines();
        triggerTime = 0f;
        isBreaking = false;   // FIX 2: allow breaking again after reset

        transform.position = originalPos;
        sr.color = originalColor;
        sr.sprite = originalSprite;
        brick.SetActive(true);

        idleCoroutine = StartCoroutine(IdleAnimation());  // FIX 1: cache on restart
    }
}