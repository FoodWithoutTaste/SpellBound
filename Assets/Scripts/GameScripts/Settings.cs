using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public TMP_Dropdown qualityDropdown;
    public Toggle fullscreenToggle;



    Resolution[] resolutions;

    private const string VolumeKey = "Volume";
    private const string QualityKey = "Quality";
    private const string FullscreenKey = "Fullscreen";
    private const string ResolutionIndexKey = "ResolutionIndex";

    private void Start()
    {
        // Load settings from PlayerPrefs
        Time.timeScale = 1;

        resolutions = Screen.resolutions;

        LoadSettings();
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate + "hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
       
    }

    public void SetResolution(int resolutionIndex)
    {
        if(resolutions.Length > resolutionIndex + 1)
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

            // Save resolution index to PlayerPrefs
            PlayerPrefs.SetInt(ResolutionIndexKey, resolutionIndex);
        }
        
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);

        // Save volume to PlayerPrefs
        PlayerPrefs.SetFloat(VolumeKey, volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);

        // Save quality index to PlayerPrefs
        PlayerPrefs.SetInt(QualityKey, qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;

        // Save fullscreen setting to PlayerPrefs
        PlayerPrefs.SetInt(FullscreenKey, isFullscreen ? 1 : 0);
    }

    // Load settings from PlayerPrefs
    private void LoadSettings()
    {
        if (PlayerPrefs.HasKey(VolumeKey))
        {
            float volume = PlayerPrefs.GetFloat(VolumeKey);
            audioMixer.SetFloat("volume", volume);
            // Set UI elements to loaded values
            volumeSlider.value = PlayerPrefs.GetFloat(VolumeKey); // Default to 0.5 if not found
        }

        if (PlayerPrefs.HasKey(QualityKey))
        {
            int qualityIndex = PlayerPrefs.GetInt(QualityKey);
            QualitySettings.SetQualityLevel(qualityIndex);
            qualityDropdown.value = PlayerPrefs.GetInt(QualityKey); // Default to medium quality if not found
        }

        if (PlayerPrefs.HasKey(FullscreenKey))
        {
            int isFullscreen = PlayerPrefs.GetInt(FullscreenKey);
            Screen.fullScreen = isFullscreen == 1;

            fullscreenToggle.isOn = PlayerPrefs.GetInt(FullscreenKey, 1) == 1; // Default to true (fullscreen) if not found
        }

        if (PlayerPrefs.HasKey(ResolutionIndexKey))
        {
            int resolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey);
           
                SetResolution(resolutionIndex);
            int loadedResolutionIndex = PlayerPrefs.GetInt(ResolutionIndexKey, 0); // Default to 0 if not found
            if (loadedResolutionIndex >= 0 && loadedResolutionIndex < resolutions.Length)
            {
                resolutionDropdown.value = loadedResolutionIndex;
                resolutionDropdown.RefreshShownValue();
            }

        }


    }
}
