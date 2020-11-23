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
        private PlayerController _playerController;

        [SerializeField] private GameObject _pauseMenuUI = null;
        [SerializeField] private SceneFaderController _sceneFaderController = null;

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

        // Freeze time, disable player movements and enable menu
        void Pause()
        {
            _pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _isGamePaused = true;
            _playerController.DisablePlayer();
        }

        #region Buttons

        // Restart time, enable player movements and disable menu
        public void Resume()
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isGamePaused = false;
            _playerController.EnablePlayer();
        }

        // Restart from last checkpoint
        public void Restart()
        {
            Events.PlayerEvents.Death.Invoke();
            Resume();
        }

        // Return to the LevelsMenu scene
        public void Exit()
        {
            Time.timeScale = 1f;
            // Deactivate all inks in the scene
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            
            // Return to the LevelsMenu scene
            _sceneFaderController.FadeTo(UnityScenes.LevelsMenu);
        }

        #endregion
    }
}
