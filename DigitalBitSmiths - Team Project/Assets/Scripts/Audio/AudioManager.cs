using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] AudioSource musicSource;   // BGM play 
    [SerializeField] AudioSource sfxSource;      // one shot sfx

    [Header("Mixer")]
    [SerializeField] AudioMixer mixer; // main mixer

    void Awake()
    {
        // Singleton to persists acros scenes
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // applying the saved volumes on start
        LoadVolumes();
    }

    // Music

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;
        if (musicSource.clip == clip && musicSource.isPlaying) return;  // already playing this track

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    // SFX

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
            sfxSource.PlayOneShot(clip);
    }

    // Volume Controll

    public void SetMasterVolume(float value)
    {
        mixer.SetFloat("MasterVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        mixer.SetFloat("MusicVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        mixer.SetFloat("SFXVolume", LinearToDecibel(value));
        PlayerPrefs.SetFloat("SFXVolume", value);
    }

    // Load the saved volumes and save them
    public void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat("MasterVolume", 1f);
        float music = PlayerPrefs.GetFloat("MusicVolume", 1f);
        float sfx = PlayerPrefs.GetFloat("SFXVolume", 1f);

        mixer.SetFloat("MasterVolume", LinearToDecibel(master));
        mixer.SetFloat("MusicVolume", LinearToDecibel(music));
        mixer.SetFloat("SFXVolume", LinearToDecibel(sfx));
    }

    // Converts a 0 to 1 slider value to decibels, t he mixer uses dB  which is apparently logarithmic 
    float LinearToDecibel(float value)
    {
        // Clamp so we never take log of 0 ;; it'll be 'silent'
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}