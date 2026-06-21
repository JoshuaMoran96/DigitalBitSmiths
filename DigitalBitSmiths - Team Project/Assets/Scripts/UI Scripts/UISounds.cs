using UnityEngine;
using UnityEngine.EventSystems;

public class UISounds : MonoBehaviour
{
    public static UISounds Instance;
   [SerializeField] AudioSource hoverSource;   // for hover and gets stopped
    [SerializeField] AudioSource clickSource;   // not stopped for hover
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    void Awake() { Instance = this; }

    public void PlayHover()
    {
        if (hoverSource != null && hoverSound != null)
        {
            hoverSource.Stop();
            hoverSource.clip = hoverSound;
            hoverSource.Play();
        }
    }

    public void StopHover()
    {
        if (hoverSource != null)
            hoverSource.Stop(); // stop on hover
    }

    public void PlayClick()
    {
        if (clickSource != null && clickSound != null)
            clickSource.PlayOneShot(clickSound); 
    }
}