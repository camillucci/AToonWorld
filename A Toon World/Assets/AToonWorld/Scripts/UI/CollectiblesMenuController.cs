using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Level;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.AToonWorld.Scripts.UI
{
    public class CollectiblesMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _collectiblesMenuUI = null;
        [SerializeField] private bool _isPositionFixed = true;
        [SerializeField] private GameObject[] _collectibleCircles = null;
        [SerializeField] private Sprite _easyCollectibleSprite = null;
        [SerializeField] private Sprite _hardCollectibleSprite = null;

        private IReadOnlyList<Collectible> _collectibles;
        private Animator _animator;
        private RectTransform _transform;

        private void Awake()
        {
            _animator = _collectiblesMenuUI.GetComponent<Animator>();
            _transform = _collectiblesMenuUI.GetComponent<RectTransform>();
        }

        public void RefreshValues()
        {
            _animator.SetTrigger(_isPositionFixed ? "IsFixed" : "IsMoving");
            _collectibles = FindObjectOfType<CollectiblesManager>().Collectibles;

            // Resize the Collectible Menu properly
            _transform.SetWidth(900f - (10 - _collectibles.Count) * 75f);
            _transform.SetX(-450f + (10 - _collectibles.Count) * 37.5f);
            for (int i = 0; i < _collectibleCircles.Length; i++)
            {
                // Enable only the last collectibles.Count circles
                if (i < _collectibles.Count)
                {
                    _collectibleCircles[i].GetComponent<Image>().sprite = _collectibles[i].IsHard ? _hardCollectibleSprite : _easyCollectibleSprite;
                    _collectibleCircles[i].SetActive(true);
                    _collectibles[i].PlayerHit += Collectible_PlayerHit;
                }
                else
                {
                    _collectibleCircles[i].SetActive(false);
                }
                _collectibleCircles[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        // Action activated when the collectible is taken
        private void Collectible_PlayerHit(Collectible collectible)
        {
            if (! _isPositionFixed)
            {
                _animator.SetTrigger("Show");
            }
            UniTask.Delay(1000).ContinueWith(() => ShowCollectibleTaken(collectible)).Forget();
        }

        // Show the gathered collectible in the UI
        private void ShowCollectibleTaken(Collectible collectible)
        {
            _collectibleCircles[collectible.CollectibleNumber - 1]
                .transform.GetChild(0).gameObject.SetActive(true);
        }

        // When the player dies, if _removeCollectiblesOnDeath, remove the collectible taken since last checkpoint
        public void ResetCollectibles(List<Collectible> collectibles)
        {
            foreach (Collectible collectible in collectibles)
            {
                _collectibleCircles[collectible.CollectibleNumber - 1]
                    .transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        // Interpolator animation
        public ScreenToWorldPointComponent GetDynamicPointConverter(int collectibleNumber)
        {
            return _collectibleCircles[collectibleNumber - 1]
                .GetComponent<ScreenToWorldPointComponent>();
        }

        // Show fixed menu when entering the pause menu
        public void ShowMenu()
        {
            _animator.SetTrigger("IsFixed");
        }

        // Display hide animation when exiting the pause menu
        public void HideMenu()
        {
            UniTask.Delay(500).ContinueWith(() =>
                _animator.SetTrigger(_isPositionFixed ? "IsFixed" : "Hide")).Forget();
        }
    }
}
