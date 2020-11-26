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
        private SceneFaderController _sceneFaderController;

        [SerializeField] private TMP_Text _totalStarsNumber = null;

        private void Awake()
        {
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
            _levels = FindObjectsOfType<LevelController>();
            _sceneFaderController = FindObjectOfType<SceneFaderController>();
        }

        // Unlock all levels that have the previous level with at least one star
        void Update()
        {
            int sum = 0;
            for(int i = 1; i < UnityScenes.Levels.Length; i++)
            {
                sum += PlayerPrefs.GetInt(UnityScenes.Levels[i], 0);
            }
            _totalStarsNumber.text = sum + "/" + (UnityScenes.Levels.Length - 1) * 3;
        }

        #region Buttons

        public void BackButton()
        {
            Time.timeScale = 1f;
            _sceneFaderController.FadeTo(UnityScenes.MainMenu);
        }

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
