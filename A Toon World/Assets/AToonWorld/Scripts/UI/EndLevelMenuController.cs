using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Extensions;
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
        private RectTransform _transform;

        [SerializeField] private GameObject _endLevelMenuUI = null;
        [SerializeField] private TMP_Text _timeText = null;
        [SerializeField] private TMP_Text _deathsText = null;
        [SerializeField] private TMP_Text _inkUsageText = null;
        [SerializeField] private Image _timeStar = null;
        [SerializeField] private Image _deathsStar = null;
        [SerializeField] private Image _inkUsageStar = null;
        [SerializeField] private Sprite _starBlankSprite = null;
        [SerializeField] private Sprite _starFullSprite = null;
        [SerializeField] private GameObject _collectibleCirclesList = null;
        [SerializeField] private GameObject[] _collectibleCircles = null;

        // Initialization when level starts
        public void RefreshValues()
        {
            // Get object used for checking achievements
            _playerController = FindObjectOfType<PlayerController>();
            _levelHandler = FindObjectOfType<LevelHandler>();
            _transform = _collectibleCirclesList.GetComponent<RectTransform>();
        }

        public void ShowEndLevelMenu()
        {
            //Request Cursor Change
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);

            // Freeze time and disable player movements
            Time.timeScale = 0f;
            _playerController.DisablePlayer();
            InGameUIController.PrefabInstance._isEndLevelMenu = true;

            // Show player achievements
            _endLevelMenuUI.SetActive(true);
            _timeText.text = _levelHandler._timeManager.getFormattedTime()
                + " / " + _levelHandler._timeManager.getFormattedAchievementTime();
            _deathsText.text = _levelHandler._deathsManager.DeathCounter.ToString()
                + " / " + _levelHandler._deathsManager.MaxDeathsForAchievement.ToString();
            _inkUsageText.text = _levelHandler._inkManager.InkUsed.ToString()
                + " / " + _levelHandler._inkManager.MaxInkForAchievement.ToString();

            // Calculate and show player stars
            int stars = 0;
            // One star if the player took less than a threshold time to complete the level
            if (_levelHandler._timeManager.GotAchievement)
            {
                _timeStar.gameObject.GetComponent<Image>().sprite = _starFullSprite;
                stars += 1;
            }
            else
            {
                _timeStar.gameObject.GetComponent<Image>().sprite = _starBlankSprite;
            }
            // One star if the player died less than a treshold amount
            if (_levelHandler._deathsManager.GotDeathsAchievement)
            {
                _deathsStar.gameObject.GetComponent<Image>().sprite = _starFullSprite;
                stars += 1;
            }
            else
            {
                _deathsStar.gameObject.GetComponent<Image>().sprite = _starBlankSprite;
            }
            // One star if the player collected all collectibles
            if (_levelHandler._inkManager.GotAchievement)
            {
                _inkUsageStar.gameObject.GetComponent<Image>().sprite = _starFullSprite;
                stars += 1;
            }
            else
            {
                _inkUsageStar.gameObject.GetComponent<Image>().sprite = _starBlankSprite;
            }
            stars = Mathf.Max(stars, PlayerPrefs.GetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, 0));

            // Resize the displayed collectible properly and show gathered collectibles
            List<Collectible> _collectibles = _levelHandler._collectiblesManager._collectibles;
            _transform.SetLeft(970f - _collectibles.Count * 70f);
            for (int i = 0; i < _collectibleCircles.Length; i++)
            {
                if (i < _collectibles.Count)
                {
                    _collectibleCircles[i].SetActive(true);
                    _collectibleCircles[i].transform.GetChild(0).gameObject.SetActive(! _collectibles[i].isActiveAndEnabled);
                }
                else
                {
                    _collectibleCircles[i].SetActive(false);
                }
            }

            // GetComponentInChildren<FeedbackButtonController>().RefreshButtons();

            // Save player progresses
            PlayerPrefs.SetInt(UnityScenes.ScenesPath + SceneManager.GetActiveScene().name, stars);
            PlayerPrefs.SetInt(UnityScenes.ScenesPath2 + SceneManager.GetActiveScene().name, stars);
        }

        // Deactivate all used inks and restart the level from the beginning
        public void Restart()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            InGameUIController.PrefabInstance.FadeTo(SceneManager.GetActiveScene().name);
        }

        // Deactivate all used inks and return to the LevelsMenu scene
        public void Continue()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }
    }
}
