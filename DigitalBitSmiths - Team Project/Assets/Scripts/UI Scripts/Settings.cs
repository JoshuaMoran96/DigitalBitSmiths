using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Settings : MonoBehaviour
{

    [SerializeField] Slider volumeSlider;
    [SerializeField] TextMeshProUGUI volumePercent;
    [SerializeField] Toggle fullscreenToggle;


    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        
        UpdateVolumeLabel(savedVolume);

        bool savedFullscreen = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        fullscreenToggle.isOn = savedFullscreen;
        Screen.fullScreen = savedFullscreen;
    }
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("MasterVolume",value);
        UpdateVolumeLabel(value);
    }

    public void SetFullscreen(bool isOn)
    {
        // Uses Borderless Fullscreen 
        Screen.fullScreenMode = isOn ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;

        // Explicitly flags the boolean state for underlying systems
        Screen.fullScreen = isOn;

        PlayerPrefs.SetInt("Fullscreen", isOn ? 1 : 0);
        PlayerPrefs.Save(); 
    }

    void UpdateVolumeLabel(float value)
    {
        if (volumePercent != null)
            volumePercent.text = Mathf.RoundToInt(value * 100) + "%";

    }



}
