using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        void Awake()
        {
            // Instantiate AudioManager and update the volume preferences
            AudioManager.PrefabInstance.MusicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.1f);
            AudioManager.PrefabInstance.SoundsVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
            
            // Instantiate Sound Effects to avoid locks during gameplay
            Audio.SoundEffects.LoadSoundEffects();

            InGameUIController.PrefabInstance.FadeInMenu();
        }

        private void Start() 
        {
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
        }

        void Update()
        {
            if (InputUtils.EnterButton)
                PlayButton();
        }

        #region Buttons

        public void PlayButton()
        {
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }

        public void QuitButton()
        {
            InGameUIController.PrefabInstance.FadeToExit();
        }

        #endregion
    }
}
