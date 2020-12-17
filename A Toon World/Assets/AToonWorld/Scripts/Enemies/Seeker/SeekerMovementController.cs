using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using Assets.AToonWorld.Scripts.Utils;
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
        [SerializeField] private float _speed = 5f;
        

        // Private Fields
        private SeekerBody _seekerBody;
        private SeekerTargetAreaController _targetAreaController;
        private GridController _gridController;
        private Transform _seekerTransform;
        private Transform _playerTransform;
        private bool _isPlayerInside;
        private Vector3 _startPosition;   
        private Animator _animator;
        private UniTask? _currentTask;
        private UniTask _cancellingTask = UniTask.CompletedTask;
        private bool _isCancellingTask;
        private int _taskCounter;

        // Initialization

        private void Awake()
        {
            _seekerBody = GetComponentInChildren<SeekerBody>();
            _seekerTransform = _seekerBody.transform;
            _targetAreaController = GetComponentInChildren<SeekerTargetAreaController>();
            _gridController = GetComponentInChildren<GridController>();
            _startPosition = _seekerTransform.position;  
            _animator = GetComponentInChildren<Animator>();     
            InitializeAreaController();
        }

        private void InitializeAreaController()
        {
            _targetAreaController.ColliderTrigger.Enter.SubscribeWithTag(UnityTag.Player, OnPlayerEnter);
            _targetAreaController.ColliderTrigger.Exit.SubscribeWithTag(UnityTag.Player, OnPlayerExit);
        }


        // Public Properties
        public bool IsMoving
        {
            get => _animator.GetBool();
            private set => _animator.SetProperty(value);
        }


        public SeekerStatus Status { get; private set; }



        // Public Methods
        public UniTask TranslateTo(Vector3 position) => _seekerTransform.MoveToAnimated(position, _speed, false);


        private void OnPlayerEnter(Collider2D collision)
        {
            _playerTransform = collision.gameObject.transform;
            _isPlayerInside = true;
            FollowPlayer().Forget();
        }

        private void OnPlayerExit(Collider2D collision)
        {           
            _isPlayerInside = false;
            GoBackToStart().Forget();
        }




        // Private Methods
        private async UniTask GoBackToStart()
        {
            async UniTask GoBackToStartTask()
            {
                IsMoving = true;
                var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, _startPosition);
                foreach (var position in path)
                    if (!_isCancellingTask)
                        await TranslateTo(position);
                    else
                        break;
                IsMoving = false;
            }

            await CancelCurrentTask();
            _currentTask = GoBackToStartTask();
        }

        private async UniTask FollowPlayer()
        {            
            async UniTask FollowTask()
            {
                IsMoving = true;
                while(!_isCancellingTask && _isPlayerInside)
                {
                    if (!IsSeekerNearToPlayer)
                    {
                        var playerPosition = _playerTransform.position;
                        var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, playerPosition);
                        var nextPositions = from pos in path where Vector2.Distance(_seekerTransform.position, pos) > _gridController.NodeRadius select pos;
                        if (nextPositions.Any())
                            await TranslateTo(nextPositions.First());
                    }
                    await UniTask.NextFrame();
                }
                IsMoving = false;
            }

            await CancelCurrentTask();
            _currentTask = FollowTask();
        }    

        private bool IsSeekerNearToPlayer => Vector2.Distance(_seekerTransform.position, _playerTransform.position) < _gridController.NodeRadius;   
        
        private async UniTask CancelCurrentTask()
        {
            while (_isCancellingTask)
                await this.NextFrame();

            if (_currentTask == null)
                return;

            _isCancellingTask = true;
            await _currentTask.Value;
            _currentTask = null;
            _isCancellingTask = false;
        }


        public enum SeekerStatus
        {
            FollowingPlayer, 
            BackToStart
        }
    }
}
