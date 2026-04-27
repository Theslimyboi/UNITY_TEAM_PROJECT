using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsMenu : MonoBehaviour
{
    [Header("Audio options")]
    public Slider volumeSlider;
    public TextMeshProUGUI volumeText;

    [Header("Resolution options")]
    public TMP_Dropdown resolutionDropdown;
    Resolution[] resolutions;

    [Header("Menu layers")]
    public GameObject mainMenuArea;
    public GameObject optionsArea;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);

        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedRes = PlayerPrefs.GetInt("SelectedRes", currentResolutionIndex);
        resolutionDropdown.value = savedRes;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetOverallVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        UpdateVolumeText(volume);
    }

    void UpdateVolumeText(float volume)
    {
        float percentage = volume * 100;
        volumeText.text = percentage.ToString("0") + "%";
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayerPrefs.SetInt("SelectedRes", resolutionIndex);
    }

    public void SetFullscreen()
    {
        Screen.fullScreen = true;
    }

    public void SetWindowed()
    {
        Screen.fullScreen = false;
    }

    public void GoBack()
    {
        optionsArea.SetActive(false);
        mainMenuArea.SetActive(true);
    }
}