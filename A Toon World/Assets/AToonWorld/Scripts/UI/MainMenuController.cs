using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        void Start()
        {
            #if UNITY_STANDALONE
                if (PlayerPrefs.GetInt("FirstLaunch", 0) == 0)
                {
                    Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
                    QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
                    PlayerPrefs.SetInt("FirstLaunch", 1);
                }
            #endif
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
