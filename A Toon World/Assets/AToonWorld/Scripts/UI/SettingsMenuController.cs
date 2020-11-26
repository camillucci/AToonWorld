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

        [SerializeField] private AudioMixer _audioMixer = null;
        [SerializeField] private TMP_Dropdown _qualityDropbox = null;
        [SerializeField] private TMP_Dropdown _resolutionDropdown = null;
        [SerializeField] private Toggle _fullscreenToggle = null;
        [SerializeField] private Slider _volumeSlider = null;

        #endregion

        private Resolution[] resolutions;

        // Refresh settings values based on settings of the last session
        void Start()
        {
            // Update quality dropbox
            _qualityDropbox.value = QualitySettings.GetQualityLevel();
            _resolutionDropdown.RefreshShownValue();

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
            
            // Update resolution dropbox
            _resolutionDropdown.ClearOptions();
            _resolutionDropdown.AddOptions(options);
            _resolutionDropdown.value = currentResolutionIndex;
            _resolutionDropdown.RefreshShownValue();

            // Update fullscreen toggle
            _fullscreenToggle.isOn = Screen.fullScreen;

            // Update volume slider
            if (_audioMixer.GetFloat("Volume", out float startingVolume))
                _volumeSlider.value = Mathf.Pow(10, startingVolume / 20);
        }

        #region Buttons

        public void SetVolume(float volume)
        {
            float newVolume = Mathf.Log10(volume)*20;
            _audioMixer.SetFloat("Volume", newVolume);
            PlayerPrefs.SetFloat("Volume", newVolume);
        }

        public void SetQuality(int qualityIndex) => QualitySettings.SetQualityLevel(qualityIndex);

        public void SetFullscreen(bool isFullscreen) => Screen.fullScreen = isFullscreen;

        public void SetResolution(int resIndex) =>
            Screen.SetResolution(resolutions[resIndex].width, resolutions[resIndex].height, Screen.fullScreen);
        
        #endregion
    }
}
