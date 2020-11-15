using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{
    public class BreakerAreaCollider : MonoBehaviour
    {
        // Private Fields
        private readonly TaggedEvent<string, Collider2D> _triggerEnter = new TaggedEvent<string, Collider2D>();
        private readonly TaggedEvent<string, Collider2D> _triggerExit = new TaggedEvent<string, Collider2D>();
        private readonly HashSet<Collider2D> _colliders = new HashSet<Collider2D>();
        private readonly List<string> _notWalkableTags = new List<string> { UnityTag.Ground };
        private BoxCollider2D _boxCollider;
        


        // Initialization
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();            
            InitializeTriggerEnter();
        }

        private void InitializeTriggerEnter()
        {           
            foreach (var tag in _notWalkableTags)
            {
                TriggerEnter.SubscribeWithTag(tag, OnNotWalkableEnter);
                TriggerExit.SubscribeWithTag(tag, OnNotWalkableExit);
            }
        }




        // Public Properties
        public ITaggedEvent<string, Collider2D> TriggerEnter => _triggerEnter;
        public ITaggedEvent<string, Collider2D> TriggerExit => _triggerExit;
        public IReadOnlyCollection<Collider2D> NotWalkableCollidersInside => _colliders;



        // Public Methods
        public void SetColliderSize(Vector2 size)
        {
            Vector2 colliderSize = _boxCollider.bounds.size;

            _boxCollider.size = new Vector2(_boxCollider.size.x / colliderSize.x * size.x, _boxCollider.size.y / colliderSize.y * size.y);
        }



        // Unity Events
        private void OnTriggerEnter2D(Collider2D collision)
        {
             _triggerEnter.InvokeWithTag(collision.gameObject.tag, collision);
        }


        // Breaker Events
        private void OnNotWalkableEnter(Collider2D collision)
        {            
            _colliders.Add(collision);
        }


        private void OnNotWalkableExit(Collider2D collision)
        {
            _colliders.Remove(collision);
        }


        // Private Methods
        private void ForceUpdateColliders()
        {            
            Vector2 center = _boxCollider.bounds.center;
            Vector2 extends = _boxCollider.bounds.extents;
            var colliders = Physics2D.OverlapBoxAll(center, extends, 0);
            foreach (var collider in _colliders)
                OnNotWalkableExit(collider);
            foreach (var collider in colliders)
                _triggerEnter.InvokeWithTag(collider.gameObject.tag, collider);
        }
    }
}