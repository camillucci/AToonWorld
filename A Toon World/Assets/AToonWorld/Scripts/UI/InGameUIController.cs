using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
        private const float _defaultSpeed = 1f;
        private const int _defaultDelay = 1000;

        [SerializeField] private GameObject _inkSelector = null;
        [SerializeField] private GameObject _inkWheel = null;
        [SerializeField] private GameObject _pauseMenuUI = null;
        [SerializeField] private GameObject _endLevelMenuUI = null;

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

        # region Faders

        // Awaitable fade in
        public async UniTask FadeIn(float fadingSpeed = _defaultSpeed)
        {
            await _sceneFaderController.FadeIn(fadingSpeed);
        }

        // Awaitable fade out
        public async UniTask FadeOut(float fadingSpeed = _defaultSpeed)
        {
            await _sceneFaderController.FadeOut(fadingSpeed);
        }

        // Awaitable fade blink
        public async UniTask FadeOutAndIn(float fadeOutSpeed = _defaultSpeed, int delayInMs = _defaultDelay, float fadeInSpeed = _defaultSpeed){
            await FadeOut(fadeOutSpeed);
            await UniTask.Delay(delayInMs);
            await FadeIn(fadeInSpeed);
        }

        // Setup the UI for the level and do a fade in
        public void FadeInLevel(float fadingSpeed = _defaultSpeed)
        {
            _inGameCanvas.gameObject.SetActive(true);
            _pauseMenuUI.SetActive(false);
            _endLevelMenuUI.SetActive(false);
            _inkSelector.SetActive(true);
            _inkWheel.SetActive(false);
            RefreshValues();
            _sceneFaderController.FadeIn(fadingSpeed).Forget();
        }

        // Refresh values that depend on a level when a new level is loaded
        private void RefreshValues()
        {
            _pauseMenuController.RefreshValues();
            _endLevelMenuController.RefreshValues();
        }

        // Setup the UI for the menu and do a fade in
        public void FadeInMenu(float fadingSpeed = _defaultSpeed)
        {
            _pauseMenuUI.SetActive(false);
            _endLevelMenuUI.SetActive(false);
            _inkSelector.SetActive(false);
            _inkWheel.SetActive(false);
            _sceneFaderController.FadeIn(fadingSpeed).ContinueWith(() =>
                _inGameCanvas.gameObject.SetActive(false)).Forget();
        }

        // Do a fade out when changing scene
        public void FadeTo(string scene, float fadingSpeed = _defaultSpeed)
        {
            _inGameCanvas.gameObject.SetActive(true);
            _sceneFaderController.FadeTo(scene, fadingSpeed).Forget();
        }

        // Do a fade out when exiting the game
        public void FadeToExit(float fadingSpeed = _defaultSpeed)
        {
            _inGameCanvas.gameObject.SetActive(true);
            _sceneFaderController.FadeExit(fadingSpeed).Forget();
        }

        #endregion

        public InkWheelController inkWheelController => _inkWheel.GetComponent<InkWheelController>();
    }
}
