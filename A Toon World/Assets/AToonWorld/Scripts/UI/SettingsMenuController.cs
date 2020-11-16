using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private TMP_Dropdown qualityDropbox;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Slider volumeSlider;

    private Resolution[] resolutions;

    void Start()
    {
        if (audioMixer.GetFloat("Volume", out float startingVolume))
            volumeSlider.value = startingVolume;

        qualityDropbox.value = QualitySettings.GetQualityLevel();
        resolutionDropdown.RefreshShownValue();

        // At run-time gather all possible resolutions
        int currentResolutionIndex = 0;
        resolutions = Screen.resolutions.Select(resolution => new Resolution {
            width = resolution.width, height = resolution.height }).Distinct().ToArray();
        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            options.Add(resolutions[i].width + " x " + resolutions[i].height);
            if (Screen.width == resolutions[i].width && Screen.height == resolutions[i].height)
                currentResolutionIndex = i;
        }
        
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetVolume(float volume) => audioMixer.SetFloat("Volume", Mathf.Log10(volume)*20);

    public void SetQuality(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);

    public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

    public void SetResolution(int resIndex) =>
        Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen);
}
