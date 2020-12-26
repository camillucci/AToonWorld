using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class DeathsManager : MonoBehaviour, IAchievementManger
    {
        [SerializeField] private int _maxDeathsForAchievement = 5;
        private int _deathCounter = 0;

        private void Start()
        {
            Events.PlayerEvents.Death.AddListener(() => OnDeath());
        }

        public void OnDeath()
        {
            _deathCounter += 1;
        }

        public int DeathCounter => _deathCounter;
        public int MaxDeathsForAchievement => _maxDeathsForAchievement;
        public bool GotAchievement() => _deathCounter <= _maxDeathsForAchievement;
        public string AchievementText() => _deathCounter.ToString() + " / " + _maxDeathsForAchievement.ToString();
    }
}
