using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.AToonWorld.Scripts.UI
{
    public class LevelController : MonoBehaviour
    {

        #region Fields

        private Button _button = null;
        [SerializeField] private int _levelNumber = -1;
        [SerializeField] private bool _isLocked = true;
        [SerializeField] private Image _unlockImage = null;
        [SerializeField] private Image[] _stars = null;
        [SerializeField] private Sprite _starBlankSprite = null;
        [SerializeField] private Sprite _starFullSprite = null;
        [SerializeField] private SceneFaderController _sceneFaderController = null;

        #endregion

        void Awake()
        {
            _button = GetComponent<Button>();
            if (_isLocked)
                _button.interactable = false;
        }

        void Update()
        {
            UpdateImages();
            UpdateStatus();
        }

        // Visualize a lock if the level is locked, the number of stars obtained if it is completed
        private void UpdateImages()
        {
            _unlockImage.gameObject.SetActive(_isLocked);
            for (int i = 0; i < _stars.Length; i++)
            {
                int starsNumber = PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber], -1);
                _stars[i].gameObject.SetActive(starsNumber >= 0);
                if (starsNumber > i)
                    _stars[i].gameObject.GetComponent<Image>().sprite = _starFullSprite;
            }
        }

        // Update the locked status based on the previous level completion
        private void UpdateStatus()
        {
            if (PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber - 1], -1) >= 0)
            {
                _isLocked = false;
                _button.interactable = true;
            }
        }

        public void StartLevel()
        {
            if (!_isLocked)
            {
                _sceneFaderController.FadeTo(UnityScenes.Levels[_levelNumber]);
            }
        }

        // Start a new game, mainly for debugging reasons
        public void ResetLevel()
        {
            if(_levelNumber != 1)
            {
                _isLocked = true;
                _button.interactable = false;
            }
            for (int i = 0; i < _stars.Length; i++)
                _stars[i].gameObject.GetComponent<Image>().sprite = _starBlankSprite;
        }

        public int LevelNumber() => _levelNumber;
    }
}
