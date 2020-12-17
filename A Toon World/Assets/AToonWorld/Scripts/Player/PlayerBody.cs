using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class PlayerBody : MonoBehaviour
    {
        // private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();
        private readonly TaggedEvent<string, Collision2D> _collisionStayHandler = new TaggedEvent<string, Collision2D>();
        private readonly ColliderTaggedEvents<Collision2D> _collisionHandler = new ColliderTaggedEvents<Collision2D>();
        private Collider2D _collider;        

        // Initialization
        void Awake()
        {
            _collider = GetComponent<Collider2D>();
        }


        // Public Properties
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;
        public IColliderTaggedEvents<Collision2D> Collision => _collisionHandler;
        public ITaggedEvent<string, Collision2D> CollisionStay => _collisionStayHandler;
        public Vector2 ColliderCenter => _collider.bounds.center;
        public Vector2 ColliderSize => _collider.bounds.size;
        public float Friction
        {
            get => _collider.sharedMaterial.friction;
            set
            {
                if (Mathf.Approximately(value, Friction))
                    return;
                _collider.sharedMaterial.friction = value;
                _collider.sharedMaterial = _collider.sharedMaterial;
            }
        }




        // Unity Events

        private void OnTriggerEnter2D(Collider2D collider)
            => _colliderTrigger.NotifyEnter(collider.gameObject.tag, collider);

        private void OnTriggerExit2D(Collider2D collider)
            => _colliderTrigger.NotifyExit(collider.gameObject.tag, collider);

        private void OnCollisionStay2D(Collision2D collision)
            => _collisionStayHandler.InvokeWithTag(collision.gameObject.tag, collision);

        private void OnCollisionEnter2D(Collision2D collision)
            => _collisionHandler.NotifyEnter(collision.gameObject.tag, collision);

        private void OnCollisionExit2D(Collision2D collision)
            => _collisionHandler.NotifyExit(collision.gameObject.name, collision);


        // Public Methods
        public void EnableCollider() => _collider.enabled = true;
        public void DisableCollider() => _collider.enabled = false;
    }
}
