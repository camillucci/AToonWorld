using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class PlayerController : MonoBehaviour
    {     

        // Private fields
        private Rigidbody2D _rigidBody;
        private PlayerInput _playerInput;
        private PlayerMovementController _playerMovementController;
        private PlayerBody _playerBodyCollider;        

        public PlayerInkController PlayerInkController { get; private set; }
        
        public bool IsImmortal { get; set; }

        // Initialization
        private void Awake()
        {
            IsImmortal = false;
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _playerMovementController = GetComponent<PlayerMovementController>();
            PlayerInkController = GetComponent<PlayerInkController>();
            _playerBodyCollider = GetComponentInChildren<PlayerBody>();
        }

        // Public Methods

        public void DisablePlayer()
        {
            _playerInput.enabled = false;
            _playerMovementController.enabled = false;
            
            if(PlayerInkController.IsDrawing)
                PlayerInkController.OnDrawReleased();
                
            PlayerInkController.enabled = false;
            _rigidBody.isKinematic = true;

            _playerBodyCollider.DisableCollider();
        }


        public void EnablePlayer()
        {
            _playerInput.enabled = true;
            _playerMovementController.enabled = true;
            PlayerInkController.enabled = true;
            _rigidBody.isKinematic = false;

            _playerBodyCollider.EnableCollider();
        }

        public UniTask MoveToPosition(Vector3 position, float speed) => transform.MoveToAnimated(position, speed);
    }
}
