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
        [SerializeField] private float _delayBeforeComeBack = 2f;
        

        // Private Fields
        private SeekerBody _seekerBody;
        private SeekerTargetAreaController _targetAreaController;
        private GridController _gridController;
        private Transform _seekerTransform;
        private Transform _playerTransform;
        private bool _isPlayerInside;
        private Vector3 _startPosition;   
        private Animator _animator;
        private SingletonTaskManager _taskManager;



        // Initialization

        private void Awake()
        {
            _seekerBody = GetComponentInChildren<SeekerBody>();
            _seekerTransform = _seekerBody.transform;
            _targetAreaController = GetComponentInChildren<SeekerTargetAreaController>();
            _gridController = GetComponentInChildren<GridController>();
            _taskManager = new SingletonTaskManager(this);
            _startPosition = _seekerTransform.position;  
            _animator = GetComponentInChildren<Animator>();     
            Status = SeekerStatus.Idle;
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
            _taskManager.ReplaceTask(() => FollowPlayer());
        }

        private void OnPlayerExit(Collider2D collision)
        {           
            _isPlayerInside = false;
            _taskManager.ReplaceTask(() => GoBackToStart((int)(_delayBeforeComeBack * 1000)));
        }



        // Private Methods

        private async UniTask GoBackToStart(int delayMs)
        {
            await this.Delay(delayMs, PlayerLoopTiming.Update, cancellationCondition: () => _taskManager.IsCancelling);
            if (_taskManager.IsCancelling)
                return;
            IsMoving = true;
            Status = SeekerStatus.BackToStart;
            var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, _startPosition);
            foreach (var position in path)
                if (!_taskManager.IsCancelling)
                    await TranslateTo(position);
                else
                    break;
            IsMoving = false;
            Status = SeekerStatus.Idle;
        }

        private async UniTask FollowPlayer()
        {            
            IsMoving = true;
            Status = SeekerStatus.FollowingPlayer;
            while(!_taskManager.IsCancelling && _isPlayerInside)
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
            Status = SeekerStatus.Idle;
        }    

        private bool IsSeekerNearToPlayer => Vector2.Distance(_seekerTransform.position, _playerTransform.position) < _gridController.NodeRadius;          

        public enum SeekerStatus
        {
            Idle,
            FollowingPlayer, 
            BackToStart
        }
    }
}
