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

    private List<Resolution> filteredResolutions = new List<Resolution>();

    [Header("Menu layers")]
    public GameObject mainMenuArea;
    public GameObject optionsArea;

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        if (volumeSlider != null) volumeSlider.value = savedVolume;
        AudioListener.volume = savedVolume;
        UpdateVolumeText(savedVolume);

        Resolution[] allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < allResolutions.Length; i++)
        {
            string option = allResolutions[i].width + " x " + allResolutions[i].height;

            if (!options.Contains(option))
            {
                options.Add(option);
                filteredResolutions.Add(allResolutions[i]);

                if (allResolutions[i].width == Screen.currentResolution.width &&
                    allResolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);

        int savedRes = PlayerPrefs.GetInt("SelectedRes", currentResolutionIndex);

        if (savedRes >= filteredResolutions.Count)
        {
            savedRes = currentResolutionIndex;
        }

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
        if (resolutionIndex < 0 || resolutionIndex >= filteredResolutions.Count) return;

        Resolution resolution = filteredResolutions[resolutionIndex];
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