using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class TimeManager : MonoBehaviour, IAchievementManger
    {
        [SerializeField] private int _maxTimeForAchievementInSeconds = 10 * 60;
        private float timeInSeconds = 0f;

        void Update()
        {
            timeInSeconds += Time.deltaTime;
        }

        public string getFormattedTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            return time.ToString(@"mm\:ss");
        }

        public string getFormattedAchievementTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(_maxTimeForAchievementInSeconds);
            return time.ToString(@"mm\:ss");
        }

        public bool GotAchievement() => timeInSeconds <= _maxTimeForAchievementInSeconds;
        public string AchievementText() => getFormattedTime() + " / " + getFormattedAchievementTime();
    }
}
