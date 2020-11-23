using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Seeker
{
    public class SeekerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed = 5f;
        

        // Private Fields
        private SeekerBody _seekerBody;
        private SeekerTargetAreaController _targetAreaController;
        private GridController _gridController;
        private Transform _seekerTransform;
        private Transform _playerTransform;
        private bool _isPlayerInside;
        private bool _canFollow;
        private Vector3 _startPosition;
        private Task _currentMovementTask = Task.CompletedTask;



        // Initialization
        private void Awake()
        {
            _seekerBody = GetComponentInChildren<SeekerBody>();
            _seekerTransform = _seekerBody.transform;
            _targetAreaController = GetComponentInChildren<SeekerTargetAreaController>();
            _gridController = GetComponentInChildren<GridController>();
            _startPosition = _seekerTransform.position;
            InitializeAreaController();
        }

        private void InitializeAreaController()
        {
            _targetAreaController.TriggerEnter.SubscribeWithTag(UnityTag.Player, OnPlayerEnter);
            _targetAreaController.TriggerExit.SubscribeWithTag(UnityTag.Player, OnPlayerExit);
        }


        // Public Methods
        public Task TranslateTo(Vector3 position) => _seekerTransform.MoveToAnimated(position, _speed, false);



        // Seeker Events
        private async void OnPlayerEnter(Collider2D collision)
        {
            _playerTransform = collision.gameObject.transform;
            _isPlayerInside = true;
            await FollowPlayer();
        }

        private async void OnPlayerExit(Collider2D collision)
        {           
            _isPlayerInside = false;
            await GoBackToStart();
        }




        // Private Methods
        private async Task GoBackToStart()
        {
            async Task GoBackToStartTask()
            {
                var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, _startPosition);
                foreach (var position in path)
                    if (_canFollow)
                        await TranslateTo(position);
                    else
                        return;
            }

            await CancelFollowTask();
            _currentMovementTask = GoBackToStartTask();
        }

        private async Task FollowPlayer()
        {            
            async Task FollowTask()
            {
                while(_canFollow && _isPlayerInside)
                {
                    if (!IsSeekerNearToPlayer)
                    {
                        var playerPosition = _playerTransform.position;
                        var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, playerPosition);
                        var nextPositions = from pos in path where Vector2.Distance(_seekerTransform.position, pos) > _gridController.NodeRadius select pos;
                        if (nextPositions.Any())
                            await TranslateTo(nextPositions.First());
                    }
                    await new WaitForEndOfFrame();
                }
            }

            await CancelFollowTask();
            _currentMovementTask = FollowTask();
        }

        private async Task CancelFollowTask()
        {
            _canFollow = false;
            await _currentMovementTask;
            _canFollow = true;
        }

        private bool IsSeekerNearToPlayer => Vector2.Distance(_seekerTransform.position, _playerTransform.position) < _gridController.NodeRadius;
    }
}
