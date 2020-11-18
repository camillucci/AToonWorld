using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private static bool _isGamePaused = false;

    public GameObject pauseMenuUI;

    void Update()
    {
        if (InputUtils.TogglePauseMenu)
        {
            if(_isGamePaused)
                Resume();
            else
                Pause();
        }
    }

    public static bool IsGamePaused => _isGamePaused;

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isGamePaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isGamePaused = true;
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(UnityScenes.MainMenu);
    }
}
