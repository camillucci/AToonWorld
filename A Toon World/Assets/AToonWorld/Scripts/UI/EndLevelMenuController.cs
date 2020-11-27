using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Level;
using Assets.AToonWorld.Scripts.Player;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.AToonWorld.Scripts.UI
{
    public class EndLevelMenuController : MonoBehaviour
    {
        private PlayerController _playerController;
        private LevelHandler _levelHandler;
        private SceneFaderController _sceneFaderController;

        [SerializeField] private Canvas _endLevelCanvas = null;
        [SerializeField] private TMP_Text _timeText = null;
        [SerializeField] private TMP_Text _deathsText = null;
        [SerializeField] private TMP_Text _collectibleText = null;
        [SerializeField] private Image _timeStar = null;
        [SerializeField] private Image _deathsStar = null;
        [SerializeField] private Image _collectiblesStar = null;
        [SerializeField] private Sprite _starFullSprite = null;

        void Awake()
        {
            _playerController = FindObjectOfType<PlayerController>();
            _levelHandler = FindObjectOfType<LevelHandler>();
            _sceneFaderController = FindObjectOfType<SceneFaderController>();
        }

        public void ShowEndLevelMenu()
        {
            // Freeze time and disable player movements
            Time.timeScale = 0f;
            _playerController.DisablePlayer();

            // Show player achievements
            _endLevelCanvas.gameObject.SetActive(true);
            _timeText.text = _levelHandler._timeManager.getFormattedTime();
            _deathsText.text = _levelHandler._deathCounter.ToString();
            _collectibleText.text = _levelHandler._collectiblesManager._currentCollectibles.ToString()
                + "/" + _levelHandler._collectiblesManager._totalCollectibles.ToString();

            // Calculate and show player stars
            int stars = 0;
            if (_levelHandler._timeManager.GotAchievement)
            {
                _timeStar.gameObject.GetComponent<Image>().sprite = _starFullSprite;
                stars += 1;
            }
            if (_levelHandler.GotDeathsAchievement)
            {
                _deathsStar.gameObject.GetComponent<Image>().sprite = _starFullSprite;
                stars += 1;
            }
            if (_levelHandler._collectiblesManager.GotAchievement)
            {
                _collectiblesStar.gameObject.GetComponent<Image>().sprite = _starFullSprite;
                stars += 1;
            }
            stars = Mathf.Max(stars, PlayerPrefs.GetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, 0));

            // Save player progresses
            PlayerPrefs.SetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, stars);
            PlayerPrefs.SetInt(UnityScenes.ScenesPath2 + SceneManager.GetActiveScene().name, stars);
        }

        // Deactivate all used inks and restart the level from the beginning
        public void Restart()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            _sceneFaderController.FadeTo(SceneManager.GetActiveScene().name);
        }

        // Deactivate all used inks and return to the LevelsMenu scene
        public void Continue()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            _sceneFaderController.FadeTo(UnityScenes.LevelsMenu);
        }
    }
}
