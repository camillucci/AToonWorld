using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class LevelsMenuController : MonoBehaviour
    {
        [SerializeField] private TMP_Text _totalStarsNumber;

        private LevelController[] _levels;

        private void Awake()
        {
            _levels = FindObjectsOfType<LevelController>();
        }

        void Update()
        {
            int sum = 0;
            for(int i = 1; i < UnityScenes.Levels.Length; i++)
            {
                sum += PlayerPrefs.GetInt(UnityScenes.Levels[i], 0);
            }
            _totalStarsNumber.text = sum + "/" + (UnityScenes.Levels.Length - 1) * 3;
        }

        public void BackButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(UnityScenes.MainMenu);
        }

        public void ResetButton()
        {
            foreach(LevelController level in _levels)
            {
                PlayerPrefs.DeleteKey(UnityScenes.Levels[level.LevelNumber()]);
                level.ResetLevel();
            }
        }
    }
}
