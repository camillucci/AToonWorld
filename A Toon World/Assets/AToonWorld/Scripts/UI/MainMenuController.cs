using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer;
        
        void Start()
        {
            float volumePref = PlayerPrefs.GetFloat("Volume", 1);
            _audioMixer.SetFloat("Volume", volumePref);
        }
        void Update()
        {
            if (InputUtils.EnterButton)
                PlayButton();
        }

        public void PlayButton()
        {
            SceneManager.LoadScene(UnityScenes.LevelsMenu);
        }

        public void QuitButton()
        {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
            
        }
    }
}
