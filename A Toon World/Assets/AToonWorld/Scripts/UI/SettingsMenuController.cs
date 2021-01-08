using Assets.AToonWorld.Scripts.Audio;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Assets.AToonWorld.Scripts.UI
{
    public class SettingsMenuController : MonoBehaviour
    {
        #region Fields

        [SerializeField] private TMP_Dropdown _qualityDropdown = null;
        [SerializeField] private TMP_Dropdown _resolutionDropdown = null;
        [SerializeField] private Toggle _fullscreenToggle = null;
        [SerializeField] private Slider _musicVolumeSlider = null;
        [SerializeField] private Slider _sfxVolumeSlider = null;

        #endregion

        private Resolution[] resolutions;

        void Start()
        {
            RefreshValues();
        }

        // Refresh settings values based on settings of the last session
        public void RefreshValues()
        {
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
            
            // Update resolution dropdown
            _resolutionDropdown.ClearOptions();
            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = currentResolutionIndex;
            _resolutionDropdown.RefreshShownValue();

            // Update quality dropdown
            _qualityDropdown.value = QualitySettings.GetQualityLevel();
            _qualityDropdown.RefreshShownValue();

            // Update fullscreen toggle
            _fullscreenToggle.isOn = Screen.fullScreen;

            // Update volume sliders
            _musicVolumeSlider.value =  AudioManager.PrefabInstance.MusicVolume;
            _sfxVolumeSlider.value =  AudioManager.PrefabInstance.SoundsVolume;
        }

        #region Buttons

        public void SetMusicVolume(float volume)
        {
            AudioManager.PrefabInstance.MusicVolume = volume;
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            AudioManager.PrefabInstance.SoundsVolume = volume;
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void SetQuality(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);

        public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

        public void SetResolution(int resIndex) =>
            Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen);
        
        #endregion
    }
}
