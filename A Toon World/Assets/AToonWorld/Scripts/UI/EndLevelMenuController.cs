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
        private List<IAchievementManger> _achievementManagers;
        private CollectiblesManager _collectiblesManager;
        private RectTransform _transform;

        [SerializeField] private GameObject _endLevelMenuUI = null;
        [SerializeField] private TMP_Text[] _achievementTexts = null;
        [SerializeField] private Image[] _medalImages = null;
        [SerializeField] private Sprite _noMedalSprite = null;
        [SerializeField] private Sprite[] _medalSprites = null;
        [SerializeField] private Sprite _easyCollectibleSprite = null;
        [SerializeField] private Sprite _hardCollectibleSprite = null;
        [SerializeField] private GameObject _collectibleCirclesList = null;
        [SerializeField] private GameObject[] _collectibleCircles = null;

        // Initialization when level starts
        public void RefreshValues()
        {
            // Get object used for checking achievements
            _playerController = FindObjectOfType<PlayerController>();
            LevelHandler levelHandler = FindObjectOfType<LevelHandler>();
            _achievementManagers = levelHandler.AchievementMangers;
            _collectiblesManager = levelHandler.CollectiblesManager;
            _transform = _collectibleCirclesList.GetComponent<RectTransform>();
        }

        public void ShowEndLevelMenu()
        {
            //Request Cursor Change
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);

            // Freeze time, disable player movements and show the menu
            Time.timeScale = 0f;
            _playerController.DisablePlayer();
            InGameUIController.PrefabInstance._isEndLevelMenu = true;
            _endLevelMenuUI.SetActive(true);
            
            // Calculate and show player medals and achievements
            // Medals are for time, deaths and ink usage
            int medals = 0;
            for (int i = 0; i < _achievementManagers.Count; i++)
            {
                _achievementTexts[i].text = _achievementManagers[i].AchievementText();
                if (_achievementManagers[i].GotAchievement())
                {
                    PlayerPrefs.SetInt(UnityScenes.LevelsPath + SceneManager.GetActiveScene().name + UnityScenes.AchievementPaths[i], 1);
                    _medalImages[i].sprite = _medalSprites[i];
                    medals += 1;
                }
                else
                {
                    _medalImages[i].sprite = _noMedalSprite;
                    medals += PlayerPrefs.GetInt(UnityScenes.LevelsPath + SceneManager.GetActiveScene().name + UnityScenes.AchievementPaths[i], 0);
                }
            }

            // Resize the displayed collectiblesList properly and show gathered collectibles
            List<Collectible> collectibles = _collectiblesManager.Collectibles;
            _transform.SetWidth(1400f - (10 - collectibles.Count) * 140f);
            for (int i = 0; i < _collectibleCircles.Length; i++)
            {
                if (i < collectibles.Count)
                {
                    _collectibleCircles[i].GetComponent<Image>().sprite = collectibles[i].IsHard ? _hardCollectibleSprite : _easyCollectibleSprite;
                    _collectibleCircles[i].SetActive(true);
                    _collectibleCircles[i].transform.GetChild(0).gameObject.SetActive(! collectibles[i].isActiveAndEnabled);
                }
                else
                {
                    _collectibleCircles[i].SetActive(false);
                }
            }

            // GetComponentInChildren<FeedbackButtonController>().RefreshButtons();

            // Save player progresses: achievements and collectibles
            medals = Mathf.Max(medals,
                PlayerPrefs.GetInt(UnityScenes.LevelsPath + SceneManager.GetActiveScene().name + UnityScenes.AchievementsPath, 0));
            int collectiblesCount = Mathf.Max(collectibles.Count,
                PlayerPrefs.GetInt(UnityScenes.LevelsPath + SceneManager.GetActiveScene().name + UnityScenes.CollectiblesPath, 0));
            PlayerPrefs.SetInt(UnityScenes.LevelsPath + SceneManager.GetActiveScene().name + UnityScenes.AchievementsPath, medals);
            PlayerPrefs.SetInt(UnityScenes.LevelsPath + SceneManager.GetActiveScene().name + UnityScenes.CollectiblesPath, collectiblesCount);
        }

        // Deactivate all used inks and restart the level from the beginning
        public void Restart()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            InGameUIController.PrefabInstance.FadeTo(SceneManager.GetActiveScene().name);
        }

        // Deactivate all used inks and return to the LevelsMenu scene
        public void ToLevelSelectionMenu()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }

        // Deactivate all used inks and start the next level
        public void toNextLevel()
        {
            ObjectPoolingManager<PlayerInkController.InkType>.Instance.DeactivateAllObjects();
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.Levels[SceneManager.GetActiveScene().buildIndex]);
        }
    }
}
