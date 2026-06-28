using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] AudioClip sceneTrack; // scene track :)

    void Start()
    {
        if (AudioManager.instance != null && sceneTrack != null)
        {
            AudioManager.instance.PlayMusic(sceneTrack); // play music
        }
    }
}