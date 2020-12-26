using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEngine;
using static PlayerInkController;

namespace Assets.AToonWorld.Scripts.Level
{
    public class InkUsageManager : MonoBehaviour, IAchievementManger
    {
        // Event called when ink is used with the type and quantity of ink used
        public static Event<float> InkQuantityChanged = new Event<float>();

        [SerializeField] private int _maxInkForAchievement = 1000;
        private float _inkUsed = 0f;

        private void Start()
        {
            InkQuantityChanged.AddListener((float inkQuantity) => OnInkQuantityChanged(inkQuantity));
        }

        private void OnInkQuantityChanged(float inkQuantity)
        {
            _inkUsed +=  inkQuantity;
        }

        public int InkUsed => (int)_inkUsed;
        public int MaxInkForAchievement => (int)_maxInkForAchievement;
        public bool GotAchievement() => _inkUsed <= _maxInkForAchievement;
        public string AchievementText() => ((int)_inkUsed).ToString() + " / " + _maxInkForAchievement.ToString();
    }
}
