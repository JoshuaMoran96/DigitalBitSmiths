using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Settings : MonoBehaviour
{

    [Header("Master")]
    [SerializeField] Slider MasterVolumeSlider;
    [SerializeField] TextMeshProUGUI MasterVolumePercent;

    [Header("Music")]
    [SerializeField] Slider MusicVolumeSlider;
    [SerializeField] TextMeshProUGUI MusicVolumePercent;
      
    [Header("SFX")]
    [SerializeField] Slider SFXVolumeSlider;
    [SerializeField] TextMeshProUGUI SFXVolumePercent;
      
    [Header("Fullscreen")]
    [SerializeField] Toggle fullscreenToggle;


    void Start()
    {
        
        // Saved Master Volume
        float savedMasterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        MasterVolumeSlider.value = savedMasterVolume;
        UpdateMasterVolumeLabel(savedMasterVolume);

         // Saved Music Volume
        float savedMusicVolume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        MusicVolumeSlider.value = savedMusicVolume;
        UpdateMusicVolumeLabel(savedMusicVolume);

         // Saved Master Volume
        float savedSFXVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        SFXVolumeSlider.value = savedSFXVolume;
        UpdateSFXVolumeLabel(savedSFXVolume);

        // Applying all the saved volumes to the mixer via AudioManager
        if (AudioManager.instance != null)
            AudioManager.instance.LoadVolumes();

        // Fullscreen
        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        Screen.fullScreen = savedFullscreen;
        
    }

    // Set Masetr Volume
    public void SetMasterVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetMasterVolume(value);
        UpdateMasterVolumeLabel(value);
    }
    // Set Music Volume
    public void SetMusicVolume(float value)
    {
        Debug.Log("SetMusicVolume CALLED with: " + value);   // ← add this
        if (AudioManager.instance != null)
            AudioManager.instance.SetMusicVolume(value);
        UpdateMusicVolumeLabel(value);
    }
    // Set SFX Volume
    public void SetSFXVolume(float value)
    {
        if (AudioManager.instance != null)
            AudioManager.instance.SetSFXVolume(value);
        UpdateSFXVolumeLabel(value);
    }

    public void SetFullscreen(bool isOn)
    {
        ApplyFullscreen(isOn);
        PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
        PlayerPrefs.Save();
    }


    // Applying the fullscreen ;; I just separated the SetFullscreen() into two like the others
    void ApplyFullscreen(bool isOn)
    {
        Screen.fullScreenMode = isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isOn;

        // This will force a windowed resolution so the window actually visibly  shrinks when off
        if (!isOn)
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
    }

    void UpdateMasterVolumeLabel(float value)
    {
        if (MasterVolumePercent != null)
            MasterVolumePercent.text = Mathf.RoundToInt(value * 100) + "%";

    }
    void UpdateMusicVolumeLabel(float value)
    {
        if (MusicVolumePercent != null)
            MusicVolumePercent.text = Mathf.RoundToInt(value * 100) + "%";

    }
    void UpdateSFXVolumeLabel(float value)
    {
        if (SFXVolumePercent != null)
            SFXVolumePercent.text = Mathf.RoundToInt(value * 100) + "%";
    }

}
