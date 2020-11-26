using System;
using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.AToonWorld.Scripts.Level
{
    public class Collectible : MonoBehaviour
    {
        public event Action<Collectible> PlayerHit;
        [SerializeField] private UnityEvent _collectibleTaken = null;

        // When hit, disable object and call subscribed events
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(UnityTag.Player))
            {
                _collectibleTaken?.Invoke();
                PlayerHit?.Invoke(this);
                gameObject.SetActive(false);
            }
        }
    }
}
