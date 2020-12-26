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
        [SerializeField] private int _totalCollectibles = -1;
        [SerializeField] private Sprite _bwImage = null;
        [SerializeField] private Image[] _medals = null;
        [SerializeField] private Sprite _noMedalSprite = null;

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

        // Update the locked status based on the previous level completion
        private void UpdateStatus()
        {
            _isLocked = PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber - 1] + UnityScenes.AchievementsPath, -1) < 0 && _levelNumber > 1;
            _button.interactable = ! _isLocked;
        }

        // Visualize the B&W image if the level is locked, the number of medals obtained if it is completed
        private void UpdateImages()
        {
            if (PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber] + UnityScenes.AchievementsPath, -1) < 0)
            {
                _image.sprite = _bwImage;
                for (int i = 0; i < _medals.Length; i++)
                {
                    _medals[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < _medals.Length; i++)
                {
                    if (PlayerPrefs.GetInt(UnityScenes.Levels[_levelNumber] + UnityScenes.AchievementPaths[i], 0) == 0)
                    {
                        _medals[i].sprite = _noMedalSprite;
                    }
                }
            }
        }

        public void StartLevel()
        {
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.Levels[_levelNumber]);
        }

        // Start a new game, mainly for debugging reasons
        public void ResetLevel()
        {
            UpdateStatus();
            UpdateImages();
        }

        public int LevelNumber => _levelNumber;
        public int TotalCollectibles => _totalCollectibles;
    }
}
