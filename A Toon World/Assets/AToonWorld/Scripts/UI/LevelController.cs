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
        private Image _image = null;
        private bool _isLocked = true;
        
        [SerializeField] private int _levelNumber = -1;
        [SerializeField] private Sprite _standardImage = null;
        [SerializeField] private Sprite _bwImage = null;
        [SerializeField] private Image[] _stars = null;
        [SerializeField] private Sprite _starBlankSprite = null;
        [SerializeField] private Sprite _starFullSprite = null;

        #endregion

        private void Awake()
        {
            _button = GetComponent<Button>();
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            _image.alphaHitTestMinimumThreshold = 0.5f;
            UpdateStatus();
            UpdateImages();
        }

        // Visualize a lock if the level is locked, the number of stars obtained if it is completed
        private void UpdateImages()
        {
            // Update background
            if (_isLocked && _levelNumber > 1)
                _image.sprite = _bwImage;
            else
                _image.sprite = _standardImage;
            
            // Update stars
            for (int i = 0; i < _stars.Length; i++)
            {
                int starsNumber = PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber], -1);
                _stars[i].gameObject.SetActive(starsNumber >= 0);
                if (starsNumber > i)
                    _stars[i].sprite = _starFullSprite;
                else
                    _stars[i].sprite = _starBlankSprite;
            }
        }

        // Update the locked status based on the previous level completion
        private void UpdateStatus()
        {
            _isLocked = PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber - 1], -1) < 0 && _levelNumber > 1;
            _button.interactable = ! _isLocked;

        }

        public void StartLevel()
        {
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.Levels[_levelNumber]);
        }

        // Start a new game, mainly for debugging reasons
        public void ResetLevel()
        {
            _image.sprite = _bwImage;
            if(_levelNumber != 1)
            {
                _isLocked = true;
                _button.interactable = false;
            }
            for (int i = 0; i < _stars.Length; i++)
                _stars[i].sprite = _starBlankSprite;
        }

        public int LevelNumber() => _levelNumber;
    }
}
