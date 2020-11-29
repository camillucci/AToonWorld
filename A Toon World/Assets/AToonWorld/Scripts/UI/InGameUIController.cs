using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public class InGameUIController : Singleton<InGameUIController>
    {
        #region Fields

        private Canvas _inGameCanvas;
        private SceneFaderController _sceneFaderController;
        private PauseMenuController _pauseMenuController;
        private EndLevelMenuController _endLevelMenuController;

        [SerializeField] GameObject _inkSelector = null;
        [SerializeField] GameObject _inkWheel = null;
        [SerializeField] GameObject _pauseMenuUI = null;
        [SerializeField] GameObject _endLevelMenuUI = null;

        #endregion

        // Initialization only the first time
        protected override void Awake()
        {
            base.Awake();
            _inGameCanvas = GetComponentInChildren<Canvas>();
            _sceneFaderController = _inGameCanvas.GetComponentInChildren<SceneFaderController>();
            _pauseMenuController = GetComponent<PauseMenuController>();
            _endLevelMenuController = GetComponent<EndLevelMenuController>();
        }

        // Setup the UI for the level and do a fade in
        public async void FadeInLevel()
        {
            _inGameCanvas.gameObject.SetActive(true);
            _pauseMenuUI.SetActive(false);
            _endLevelMenuUI.SetActive(false);
            _inkSelector.SetActive(true);
            _inkWheel.SetActive(false);
            RefreshValues();
            await _sceneFaderController.FadeIn();
        }

        // Setup the UI for the menu and do a fade in
        public async void FadeInMenu()
        {
            _pauseMenuUI.SetActive(false);
            _endLevelMenuUI.SetActive(false);
            _inkSelector.SetActive(false);
            _inkWheel.SetActive(false);
            await _sceneFaderController.FadeIn();
            _inGameCanvas.gameObject.SetActive(false);
        }

        // Refresh values that depend on a level when a new level is loaded
        public void RefreshValues()
        {
            _pauseMenuController.RefreshValues();
            _endLevelMenuController.RefreshValues();
        }

        // Do a fade out when changing scene
        public void FadeTo(string scene)
        {
            _inGameCanvas.gameObject.SetActive(true);
            _sceneFaderController.FadeTo(scene);
        }

        // Do a fade out when exiting the game
        public void FadeToExit()
        {
            _inGameCanvas.gameObject.SetActive(true);
            _sceneFaderController.FadeToExit();
        }
    }
}
