using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class LevelsMenuController : MonoBehaviour
    {
        private LevelController[] _levels;

        [SerializeField] private TMP_Text _totalStarsNumber = null;
        [SerializeField] private TMP_Text _totalCollectiblesNumber = null;

        private void Awake()
        {
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
        }

        // Initialization
        private void Start()
        {
            _levels = FindObjectsOfType<LevelController>();
            UpdateTotalMedals();
            UpdateTotalCollectibles();
            InGameUIController.PrefabInstance.FadeInMenu();
        }

        // Visualize the total number of medals collected
        private void UpdateTotalMedals()
        {
            int gathered = 0, total = (UnityScenes.Levels.Length - 1) * 3;
            foreach(LevelController level in _levels)
            {
                gathered += PlayerPrefs.GetInt(UnityScenes.Levels[level.LevelNumber] + UnityScenes.AchievementsPath, 0);
            }
            _totalStarsNumber.text = gathered + " / " + total;
        }

        // Visualize the total number of collectibles gathered
        private void UpdateTotalCollectibles()
        {
            int gathered = 0, total = 0;
            foreach(LevelController level in _levels)
            {
                gathered += PlayerPrefs.GetInt(UnityScenes.Levels[level.LevelNumber] + UnityScenes.CollectiblesPath, 0);
                total += level.TotalCollectibles;
            }
            _totalCollectiblesNumber.text = gathered + " / " + total;
        }

        #region Buttons

        // Return to main menu
        public void BackButton()
        {
            Time.timeScale = 1f;
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.MainMenu);
        }

        // Reset all progression
        public void ResetButton()
        {
            foreach(LevelController level in _levels)
            {
                PlayerPrefs.DeleteKey(UnityScenes.Levels[level.LevelNumber] + UnityScenes.AchievementsPath);
                PlayerPrefs.DeleteKey(UnityScenes.Levels[level.LevelNumber] + UnityScenes.AchievementsPath);
                for (int i = 0; i < UnityScenes.AchievementPaths.Length; i++)
                {
                    PlayerPrefs.DeleteKey(UnityScenes.Levels[level.LevelNumber] + UnityScenes.AchievementPaths[i]);
                }
                level.ResetLevel();
            }
        }

        #endregion
    }
}
