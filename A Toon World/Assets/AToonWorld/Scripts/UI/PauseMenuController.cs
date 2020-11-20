using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class PauseMenuController : MonoBehaviour
    {
        private static bool _isGamePaused = false;

        [SerializeField] private GameObject _pauseMenuUI;
        [SerializeField] private TMP_Dropdown _qualityDropDown;

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
            _qualityDropDown.Select();
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
            Resume();
        }

        public void ExitGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(UnityScenes.LevelsMenu);
        }
    }
}
