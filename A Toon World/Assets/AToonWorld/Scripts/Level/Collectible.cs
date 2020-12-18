using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.AToonWorld.Scripts.Level
{
    public class Collectible : MonoBehaviour
    {
        public event Action<Collectible> PlayerHit;

        [SerializeField] private int _collectibleNumber = -1;
        [SerializeField] private UnityEvent _collectibleTaken = null;

        //Effects Properties
        [SerializeField] private int _collectibleSparks = 5;
        [SerializeField] private Sprite _collectibleSparkSprite = null;
        [SerializeField] private Vector3 _collectibleSparkScale = Vector3.one;
        [SerializeField] private Color _collectibleSparkColor = Color.white;

        // When hit, disable object and call subscribed events
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
            {
                _collectibleTaken?.Invoke();
                PlayerHit?.Invoke(this);
                gameObject.SetActive(false);

                // Interpolator animation
                InGameUIController.PrefabInstance.WorldToUIEffects.SpawnEffects(
                    _collectibleSparks, _collectibleSparkSprite, _collectibleSparkScale, _collectibleSparkColor,
                    this.transform, InGameUIController.PrefabInstance.CollectibleMenu.GetDynamicPointConverter(_collectibleNumber));
            }
        }

        public int CollectibleNumber => _collectibleNumber;
    }
}
