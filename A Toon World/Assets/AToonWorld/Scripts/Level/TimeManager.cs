using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] private float _achievementTimeInSeconds = 10 * 60f;
        private float timeInSeconds = 0f;

        void Update()
        {
            timeInSeconds += Time.deltaTime;
        }

        public string getFormattedTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(timeInSeconds);
            return time.ToString(@"hh\:mm\:ss\:fff");
        }

        public bool GotAchievement => timeInSeconds <= _achievementTimeInSeconds;
    }
}
