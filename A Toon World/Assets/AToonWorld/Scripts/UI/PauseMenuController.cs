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
        private GameState _gameState = GameState.InGame;
        private PlayerController _playerController;

        [SerializeField] private GameObject _pauseMenuUI = null;
        [SerializeField] private GameObject _settingsMenuUI = null;
        [SerializeField] private GameObject _controlsMenuUI = null;

        void Update()
        {
            if (InputUtils.ToggleSettingsMenu)
            {
                if (_gameState == GameState.InSettingsMenu)
                {
                    Resume();
                }
                else if (InGameUIController.PrefabInstance.CanPause)
                {
                    ShowSettingsMenu();
                    Pause();
                }
            }
            else if (InputUtils.ToggleControlsMenu)
            {
                if (_gameState == GameState.InControlsMenu)
                {
                    Resume();
                }
                else if (InGameUIController.PrefabInstance.CanPause)
                {
                    ShowControlsMenu();
                    Pause();
                }
            }
        }

        // Initialization when level starts
        public void RefreshValues()
        {
            _playerController = FindObjectOfType<PlayerController>();
            _gameState = GameState.InGame;
            _settingsMenuUI.GetComponent<SettingsMenuController>().RefreshValues();
        }

        // Pause game and enable pause menu
        public void ShowSettingsMenu()
        {
            _settingsMenuUI.SetActive(true);
            _controlsMenuUI.SetActive(false);
            _gameState = GameState.InSettingsMenu;
        }

        // Pause and enable controls menu
        public void ShowControlsMenu()
        {
            _settingsMenuUI.SetActive(false);
            _controlsMenuUI.SetActive(true);
            _gameState = GameState.InControlsMenu;
        }

        // Freeze time, disable player movements
        private void Pause()
        {
            _pauseMenuUI.SetActive(true);
            Time.timeScale = 0f;
            _playerController.DisablePlayer();
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
            InGameUIController.PrefabInstance.CollectibleMenu.ShowMenu();
        }

        #region Buttons

        // Restart time, enable player movements and disable menu
        public void Resume()
        {
            _pauseMenuUI.SetActive(false);
            Time.timeScale = 1f;
            _gameState = GameState.InGame;
            _playerController.EnablePlayer();
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Game);
            InGameUIController.PrefabInstance.CollectibleMenu.HideMenu();
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

        private enum GameState
        {
            InGame,
            InSettingsMenu,
            InControlsMenu,
        }
    }
}
