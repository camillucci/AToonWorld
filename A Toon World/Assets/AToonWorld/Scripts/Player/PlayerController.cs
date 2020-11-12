using Assets.AToonWorld.Scripts.Extensions;
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
        // Editor Fields
        [SerializeField] private float _respawnSpeed = 10f;


        // Private fields
        private Rigidbody2D _rigidBody;
        private PlayerInput _playerInput;
        private PlayerMovementController _playerMovementController;
        private PlayerInkController _playerInkController;

        // Initialization
        private void Awake()
        {
            _rigidBody = GetComponent<Rigidbody2D>();
            _playerInput = GetComponent<PlayerInput>();
            _playerMovementController = GetComponent<PlayerMovementController>();
            _playerInkController = GetComponent<PlayerInkController>();
        }


        // Public Methods

        public void DisablePlayer()
        {
            _playerInput.enabled = false;
            _playerMovementController.enabled = false;
            _playerInkController.enabled = false;
            _rigidBody.isKinematic = true;
        }


        public void EnablePlayer()
        {
            _playerInput.enabled = true;
            _playerMovementController.enabled = true;
            _playerInkController.enabled = true;
            _rigidBody.isKinematic = false;
        }

        public Task MoveToPosition(Vector3 position) => transform.MoveToAnimated(position, _respawnSpeed);
    }
}
