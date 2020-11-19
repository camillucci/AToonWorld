using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    void Start()
    {
        #if UNITY_STANDALONE
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            QualitySettings.SetQualityLevel(QualitySettings.names.Length - 1);
        #endif
    }

    void Update()
    {
        if (InputUtils.EnterButton)
            PlayButton();
    }

    public void PlayButton()
    {
        SceneManager.LoadScene(UnityScenes.LevelMenu);
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
