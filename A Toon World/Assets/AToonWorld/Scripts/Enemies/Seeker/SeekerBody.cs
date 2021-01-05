using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Seeker
{
    public class SeekerBody : MonoBehaviour, IKillable
    {
        // Private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();
        private SeekerMovementController _seekerMovementController;
        private Transform _seekerTransform;
        private Vector3 _startPosition;
        private Transform _playerTransform;

        // Initialization
        private void Awake()
        {
            _seekerMovementController = GetComponentInParent<SeekerMovementController>();
            _seekerTransform = this.transform;
            _startPosition = this.transform.position;
            _colliderTrigger.Enter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
        }

        private void Start()
        {
            _playerTransform = FindObjectOfType<PlayerController>().gameObject.transform;
        }

        private void Update()
        {
            if (_seekerMovementController.Status == SeekerMovementController.SeekerStatus.BackToStart)
                _seekerTransform.rotation = LookAt(_seekerTransform.position, _startPosition);
            else
                _seekerTransform.rotation = LookAt(_seekerTransform.position, _playerTransform.position);
        }

        private Quaternion LookAt(Vector2 me, Vector2 target)
        {
            Vector2 direction = target - me;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            return Quaternion.AngleAxis(angle, Vector3.forward);
        }

        // Public Properties
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;



        // UNity Events
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _colliderTrigger.NotifyEnter(collision.gameObject.tag, collision);            
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _colliderTrigger.NotifyExit(collision.gameObject.tag, collision);
        }


        // Seeker Events

        private void OnDrawingEnter(Collider2D collision)
        {
            collision.gameObject.SetActive(false);
        }

        
        public void Kill()
        {
            this.transform.parent.GetComponent<IKillable>()?.Kill();
        }
    }
}
