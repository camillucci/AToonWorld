using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Seeker
{
    public class SeekerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed;
        

        // Private Fields
        private SeekerBody _seekeBody;
        private SeekerTargetAreaController _targetAreaController;
        private GridController _gridController;
        private Transform _seekerTransform;
        private Transform _playerTransform;
        private bool _isPlayerInside;
        private bool _canFollow;
        private Vector3 _startPosition;        
        private UniTask? _currentMovementTask = UniTask.CompletedTask;



        // Initialization

        private void Awake()
        {
            _seekeBody = GetComponentInChildren<SeekerBody>();
            _seekerTransform = _seekeBody.transform;
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
        public UniTask TranslateTo(Vector3 position) => _seekerTransform.MoveToAnimated(position, _speed, false);



        private void OnPlayerEnter(Collider2D collision)
        {
            _playerTransform = collision.gameObject.transform;
            _isPlayerInside = true;
            FollowPlayer().Forget();
        }

        private async void OnPlayerExit(Collider2D collision)
        {           
            _isPlayerInside = false;
            await GoBackToStart();
        }




        // Private Methods
        private async UniTask GoBackToStart()
        {
            async UniTask GoBackToStartTask()
            {
                var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, _startPosition);
                foreach (var position in path)
                    if (_canFollow)
                        await TranslateTo(position).WithCancellation(this.GetCancellationTokenOnDestroy());
                    else
                        return;
            }

            await CancelFollowTask();
            _currentMovementTask = GoBackToStartTask();
        }

        private async UniTask FollowPlayer()
        {            
            async UniTask FollowTask()
            {
                while(_canFollow && _isPlayerInside)
                {
                    if (!IsSeekerNearToPlayer)
                    {
                        var playerPosition = _playerTransform.position;
                        var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, playerPosition);
                        var nextPositions = from pos in path where Vector2.Distance(_seekerTransform.position, pos) > _gridController.NodeRadius select pos;
                        if (nextPositions.Any())
                            await TranslateTo(nextPositions.First()).WithCancellation(this.GetCancellationTokenOnDestroy());
                    }
                    await UniTask.WaitForEndOfFrame();
                }
            }

            await CancelFollowTask();
            _currentMovementTask = FollowTask();
        }

        private async UniTask CancelFollowTask()
        {
            _canFollow = false;
            if (_currentMovementTask != null)
            {
                var task = _currentMovementTask.Value;
                _currentMovementTask = null;
                await task;
            }
            _canFollow = true;
        }

        private bool IsSeekerNearToPlayer => Vector2.Distance(_seekerTransform.position, _playerTransform.position) < _gridController.NodeRadius;
    }
}
