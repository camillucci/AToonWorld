using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.AToonWorld.Scripts.Level
{
    public class CheckPoint : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private int _checkPointNumber = -1;
        [SerializeField] private bool _isStart = false;


        
        // Private fields
        private bool _hit;



        // Initialization
        private void Awake()
        {
            _transform = transform;
            _colliderTrigger.Enter.SubscribeWithTag(UnityTag.Player, OnPlayerHit);
        }



        // Events
        public event Action<CheckPoint> PlayerHit;
        [SerializeField] private UnityEvent _checkpointTaken = null;
        [SerializeField] private UnityEvent _playerSpawning = null;
        [SerializeField] private UnityEvent _playerSpawned = null;



        // Private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();
        private Transform _transform;




        // Public Properties
        public Vector3 Position => _transform.position;
        public int CheckPointNumber => _checkPointNumber;
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;

        public bool Hit => _hit || IsStart;
        public bool IsStart => _isStart;




        // Unity Events
        private void OnTriggerEnter2D(Collider2D collision) => _colliderTrigger.NotifyEnter(collision.gameObject.tag, collision);

        // CheckPoint events        
        private void OnPlayerHit(Collider2D collision)
        {
            if (Hit)
                return;

            _hit = true;
            
            //Events
            #if AnaliticsEnabled
                Events.AnaliticsEvents.Checkpoint.Invoke(new Analitic(_checkPointNumber));
            #endif
            Events.LevelEvents.CheckpointReached.Invoke(_checkPointNumber);
            _checkpointTaken?.Invoke();
            PlayerHit?.Invoke(this);
        }

        public void OnPlayerRespawnStart()
        {
            _playerSpawning?.Invoke();
        }

        public void OnPlayerRespawnEnd()
        {
            _playerSpawned?.Invoke();
        }
    }
}
