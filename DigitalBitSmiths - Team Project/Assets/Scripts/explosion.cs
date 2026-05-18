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
        flash.Play();
        sparks.Play();
        fire.Play();
        smoke.Play();
    }
}
