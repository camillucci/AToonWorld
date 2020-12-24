using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class DeathsManager : MonoBehaviour
    {
        [SerializeField] private int _maxDeathsForAchievement = 5;
        private int _deathCounter = 0;

        public void OnDeath()
        {
            _deathCounter += 1;
        }

        public int DeathCounter => _deathCounter;
        public int MaxDeathsForAchievement => _maxDeathsForAchievement;
        public bool GotDeathsAchievement => _deathCounter <= _maxDeathsForAchievement;
    }
}
