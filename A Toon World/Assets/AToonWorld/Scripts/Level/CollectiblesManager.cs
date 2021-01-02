using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class CollectiblesManager : MonoBehaviour
    {
        private List<Collectible> _collectibles = new List<Collectible>();
        private int _totalCollectibles;
        private int _currentCollectibles;
        private List<Collectible> _collectiblesSinceLastCheckpoint =  new List<Collectible>();

        [SerializeField] private bool _removeCollectiblesOnDeath = true;

        // Initialization
        private void Start()
        {
            // Search and setup all the collectibles in the level
            Collectible[] collectibles = FindObjectsOfType<Collectible>();
            foreach (var collectible in collectibles)
                SetupCheckPoint(collectible);
            _collectibles.AddRange(collectibles);
            _collectibles.Sort((x, y) => x.CollectibleNumber.CompareTo(y.CollectibleNumber));
            _totalCollectibles = _collectibles.Count;

            // Refresh the collectibles menu to show the collectibles
            InGameUIController.PrefabInstance.CollectibleMenu.RefreshValues(_collectibles);
        }

        private void SetupCheckPoint(Collectible collectible)
        {
            collectible.PlayerHit += Collectible_PlayerHit;
        }

        private void Collectible_PlayerHit(Collectible collectible)
        {
            _currentCollectibles += 1;
            _collectiblesSinceLastCheckpoint.Add(collectible);
        }
        
        // If the player dies, reset all collectibles obtained after last checkpoint
        public void OnPlayerRespawn()
        {
            if (_removeCollectiblesOnDeath)
            {
                InGameUIController.PrefabInstance.GetComponent<CollectiblesMenuController>()
                    .ResetCollectibles(_collectiblesSinceLastCheckpoint);
                _currentCollectibles -= _collectiblesSinceLastCheckpoint.Count;
                foreach (Collectible collectible in _collectiblesSinceLastCheckpoint)
                    collectible.gameObject.SetActive(true);
                _collectiblesSinceLastCheckpoint.Clear();
            }
        }

        // If a player hit a new checkpoint, save all gathered collectibles
        public void OnPlayerHitCheckpoint(CheckPoint checkpoint)
        {
            _collectiblesSinceLastCheckpoint.Clear();
        }

        public List<Collectible> Collectibles => _collectibles;
        public int TotalCollectibles => _totalCollectibles;
        public int CurrentCollectibles => _currentCollectibles;
        public bool GotAchievement => _currentCollectibles == _totalCollectibles;
    }
}
