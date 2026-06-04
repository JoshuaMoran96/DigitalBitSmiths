using System.Collections;
using UnityEngine;

public class explosion : MonoBehaviour
{

    [SerializeField] ParticleSystem flash;
    [SerializeField] ParticleSystem sparks;
    [SerializeField] ParticleSystem smoke;
    [SerializeField] ParticleSystem fire;

    // Update is called once per frame
    void Start()
    {
        StartCoroutine(destroyExplosion());   
    }

    //created this IEnumerator to allow the animation to play and destroy the Explosion game object, that was previously staying forever.
    IEnumerator destroyExplosion()
    {
        flash.Play();
        sparks.Play();
        fire.Play();
        smoke.Play();

        yield return new WaitForSeconds(0.5f);

        Destroy(gameObject);
    }
}
