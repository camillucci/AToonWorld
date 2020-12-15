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
        private Collider2D _collider;
        private PhysicsMaterial2D _material;
        private float _savedFriction;
        private IColliderTaggedEvents<Collision2D> _collisionHandler;


        // Initialization
        void Awake()
        {
            this._collider = GetComponent<Collider2D>();
            _material = _collider.sharedMaterial;
        }


        // Public Properties
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;
        public ITaggedEvent<string, Collision2D> CollisionStay => _collisionStayHandler;
        
        public bool FrictionEnabled
        {
            get => !Mathf.Approximately(_material.friction, 0);
            set
            {
                if (value == FrictionEnabled)
                    return;

                if (!value)
                {
                    _savedFriction = _material.friction;
                    _material.friction = 0;
                }
                else
                    _material.friction = _savedFriction;

                // Force unity to know the change
                _collider.enabled = false;
                _collider.enabled = true;
            }
        }

  
        // Unity Events

        private void OnTriggerEnter2D(Collider2D collider)
            => _colliderTrigger.NotifyEnter(collider.gameObject.tag, collider);

        private void OnTriggerExit2D(Collider2D collider)
            => _colliderTrigger.NotifyExit(collider.gameObject.tag, collider);

        private void OnCollisionStay2D(Collision2D collision)
            => _collisionStayHandler.InvokeWithTag(collision.gameObject.tag, collision);



        // Public Methods
        public void EnableCollider() => _collider.enabled = true;
        public void DisableCollider() => _collider.enabled = false;        
    }
}
