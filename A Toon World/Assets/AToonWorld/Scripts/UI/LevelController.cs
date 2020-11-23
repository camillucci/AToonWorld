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
        private Button _button;
        [SerializeField] private int _levelNumber;
        [SerializeField] private bool _isLocked;
        [SerializeField] private Image _unlockImage;
        [SerializeField] private Image[] _stars;
        [SerializeField] private Sprite _starBlankSprite;
        [SerializeField] private Sprite _starFullSprite;

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

        private void UpdateImages()
        {
            _unlockImage.gameObject.SetActive(_isLocked);
            for (int i = 0; i < _stars.Length; i++)
            {
                _stars[i].gameObject.SetActive(!_isLocked);
                if (i < PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber], 0))
                    _stars[i].gameObject.GetComponent<Image>().sprite = _starFullSprite;
            }
        }

        private void UpdateStatus()
        {
            if (PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber - 1]) > 0)
            {
                _isLocked = false;
                _button.interactable = true;
            }
        }

        public void StartLevel()
        {
            if (!_isLocked)
            {
                SceneManager.LoadScene(UnityScenes.Levels[_levelNumber]);
            }
        }

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
