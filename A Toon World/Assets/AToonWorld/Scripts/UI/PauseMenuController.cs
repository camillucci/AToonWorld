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

        // Initialization
        private void Start()
        {
            RefreshValues();
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

        public void RefreshValues()
        {
            _playerController = FindObjectOfType<PlayerController>();
        }

        public static bool IsGamePaused => _isGamePaused;

        // Freeze time, disable player movements and enable menu
        void Pause()
        {
            if (! InGameUIController.PrefabInstance.CanPause)
                return;
            _pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _isGamePaused = true;
            _playerController.DisablePlayer();
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
        }

        #region Buttons

        // Restart time, enable player movements and disable menu
        public void Resume()
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _isGamePaused = false;
            _playerController.EnablePlayer();
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Game);
        }

        // Restart from last checkpoint
        public void Restart()
        {
            // Deactivate all inks in the scene
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();

            // Return to the LevelsMenu scene
            InGameUIController.PrefabInstance.FadeTo(SceneManager.GetActiveScene().name);
        }

        // Return to the LevelsMenu scene
        public void Exit()
        {
            // Deactivate all inks in the scene
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            
            // Return to the LevelsMenu scene
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }

        #endregion
    }
}
