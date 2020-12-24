using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Level;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public class CollectiblesMenuController : MonoBehaviour
    {
        [SerializeField] private GameObject _collectiblesMenuUI = null;
        [SerializeField] private bool _isPositionFixed = true;
        [SerializeField] private GameObject[] _collectibleCircles = null;

        private int _totalCollectibles = -1;
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
            List<Collectible> collectibles = FindObjectOfType<CollectiblesManager>()._collectibles;

            // Resize the Collectible Menu properly
            _totalCollectibles = collectibles.Count;
            _transform.SetLeft(1025f + (10 - collectibles.Count) * 75f);
            for (int i = 0, j = 0; i < _collectibleCircles.Length; i++)
            {
                // Enable only the last collectibles.Count circles
                if (i < 10 - collectibles.Count)
                {
                    _collectibleCircles[i].SetActive(false);
                }
                else
                {
                    _collectibleCircles[i].SetActive(true);
                    collectibles[j].PlayerHit += Collectible_PlayerHit;
                    j++;
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
            _collectibleCircles[10 - _totalCollectibles + collectible.CollectibleNumber - 1]
                .transform.GetChild(0).gameObject.SetActive(true);
        }

        // When the player dies, if _removeCollectiblesOnDeath, remove the collectible taken since last checkpoint
        public void ResetCollectibles(List<Collectible> collectibles)
        {
            foreach (Collectible collectible in collectibles)
            {
                _collectibleCircles[10 - _totalCollectibles + collectible.CollectibleNumber - 1]
                    .transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        // Interpolator animation
        public ScreenToWorldPointComponent GetDynamicPointConverter(int collectibleNumber)
        {
        return _collectibleCircles[10 - _totalCollectibles + collectibleNumber - 1]
            .GetComponent<ScreenToWorldPointComponent>();
        }
    }
}
