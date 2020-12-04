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

        private void Awake()
        {
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
        }

        // Initialization
        private void Start()
        {
            _levels = FindObjectsOfType<LevelController>();
            InGameUIController.PrefabInstance.FadeInMenu();
        }

        // Visualize the total number of star collected
        void Update()
        {
            int sum = 0;
            for(int i = 1; i < UnityScenes.Levels.Length; i++)
            {
                sum += PlayerPrefs.GetInt(UnityScenes.Levels[i], 0);
            }
            _totalStarsNumber.text = sum + " / " + (UnityScenes.Levels.Length - 1) * 3;
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
                PlayerPrefs.DeleteKey(UnityScenes.Levels[level.LevelNumber()]);
                level.ResetLevel();
            }
        }

        #endregion
    }
}
