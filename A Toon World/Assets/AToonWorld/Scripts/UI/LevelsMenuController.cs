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

        void Update()
        {
            int sum = 0;
            for(int i = 0; i < UnityScenes.Levels.Length; i++)
            {
                sum += PlayerPrefs.GetInt(UnityScenes.Levels[i], 0);
            }
            _totalStarsNumber.text = sum + "/" + UnityScenes.Levels.Length * 3;
        }

        public void BackButton()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(UnityScenes.MainMenu);
        }

        public void ResetButton()
        {
            for(int i = 1; i < UnityScenes.Levels.Length; i++)
            {
                PlayerPrefs.DeleteKey(UnityScenes.Levels[i]);
            }
        }
    }
}
