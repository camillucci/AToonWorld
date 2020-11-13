using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class DeathObserver : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _maxFallDistanceBeforeDeath = 3;


        // Private Fields
        private PlayerMovementController _playerMovementController;
        private Transform _playerTransform;
        private Vector3 _previousGroundedPosition;        
        
        
        // Initialization
        private void Awake()
        {
            _playerMovementController = FindObjectOfType<PlayerMovementController>();            
        }

        private void Start()
        {
            IsImmortal = false;
            _playerTransform = _playerMovementController.transform;
            _previousGroundedPosition = _playerTransform.position;

            SubscribeToFallDeathEvents();
        }


        // Public Events
        public event Action PlayerDead;



        // Public Properties        
        public bool IsImmortal { get; set; }



        


        // Private Methods
        private void SubscribeToFallDeathEvents()
        {
            Debug.Log("subscribed");
            var fallDeathTagsToCheck = new string[] { UnityTag.ClimbingWall, UnityTag.Drawing, UnityTag.Ground };

            foreach(var tag in fallDeathTagsToCheck)
            {
                _playerMovementController.PlayerFeet.TriggerEnter.SubscribeWithTag(tag, CheckFallDeath);
                _playerMovementController.PlayerFeet.TriggerExit.SubscribeWithTag(tag, CheckFallDeath);
            }
        }

      
        private void CheckFallDeath(Collider2D collision)
        {
            var (previousPos, currentPos) = (_previousGroundedPosition, _playerTransform.position);
            _previousGroundedPosition = currentPos;
            if (IsFallDeath(previousPos, currentPos))
                InvokeDeathEvent();
            _previousGroundedPosition = currentPos;
        }


        private bool IsFallDeath(Vector3 start, Vector3 end) 
            => start.y - end.y > _maxFallDistanceBeforeDeath;


        private void InvokeDeathEvent()
        {
            if (!IsImmortal)
                PlayerDead?.Invoke();
        }
    }
}
