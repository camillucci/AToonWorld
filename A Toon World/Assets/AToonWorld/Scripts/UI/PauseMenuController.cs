﻿using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    private static bool _isGamePaused = false;

    [SerializeField] private GameObject _pauseMenuUI;

    private PlayerController _playerController;

    void Awake()
    {
        _playerController = FindObjectOfType<PlayerController>();
    }

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

    void Pause()
    {
        _pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        _isGamePaused = true;
        _playerController.DisablePlayer();
    }

    public void Resume()
    {
        _pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        _isGamePaused = false;
        _playerController.EnablePlayer();
    }

    public void Restart()
    {
        Events.PlayerEvents.Death.Invoke();
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(UnityScenes.MainMenu);
    }
}
