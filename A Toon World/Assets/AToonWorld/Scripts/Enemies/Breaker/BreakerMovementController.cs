using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using Assets.AToonWorld.Scripts.PathFinding.Utils;
using Assets.AToonWorld.Scripts.Utils;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{       
    public class BreakerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 1f;
        [SerializeField] private Transform _propulsionLocation = null;


        // Private Fields
        public BreakerDrawingHandler _breakerDrawingHandler;
        private BreakerBody _breakerBody;
        private BreakerTargetAreaHandler _breakerAreaHandler;
        private Transform _breakerTransform;                
        private GridController _gridController;
        private Animator _animator;
        private Vector3 _propulsionDirection;
        private Quaternion _breakerIdleRotation;
        private UniTask? _followPathTask = UniTask.CompletedTask;
        private bool _seekerActive;

        public BreakerMovementController(bool seekerActive)
        {
            _seekerActive = seekerActive;
        }

        private bool _linesUpdated;

        // Private Properties
        private bool LinesUpdated 
        { 
            get => _linesUpdated;  
            set
            {
                _linesUpdated = value;
                if (value && !_seekerActive)
                    SeekerMovement().WithCancellation(this.GetCancellationTokenOnDestroy()).Forget();
            }
        }

        // Initialization
        private void Awake()
        {
            _gridController = GetComponent<GridController>();
            _breakerBody = GetComponentInChildren<BreakerBody>();
            _animator = GetComponentInChildren<Animator>();
            _breakerAreaHandler = GetComponentInChildren<BreakerTargetAreaHandler>();
            _breakerTransform = _breakerBody.transform;
            _breakerIdleRotation = _breakerTransform.rotation;
            _breakerDrawingHandler = new BreakerDrawingHandler(_breakerTransform.position);
            _propulsionDirection = (_propulsionLocation.position - _breakerTransform.position).normalized;
            BreakerTargetAreaHandlerInitialization();
        }

        private void BreakerTargetAreaHandlerInitialization()
        {
            _breakerAreaHandler.NewLineInRange += OnNewLineInRange;
            _breakerAreaHandler.LineOutOfRange += OnLineOutOfRange; 
        }

        // Public Properties
        public INode CurrentNode => _gridController.WorldPointToNode(_breakerTransform.position);



        // Public Methods
        public UniTask TranslateTo(Vector3 position) => _breakerTransform.MoveToAnimated(position, _speed, false);
        public UniTask RotateTowards(Quaternion rotation, CancellationToken cancellationToken) => _breakerTransform.RotateTowardsAnimatedWithCancellation(rotation, cancellationToken, _rotationSpeed, true);
        public UniTask TranslateTo(INode node) => _breakerTransform.MoveToAnimated(_gridController.NodeToWorldPoint(node), _speed, false);
        public void TeleportTo(INode node) => _breakerTransform.position = _gridController.NodeToWorldPoint(node);
        public void TeleportTo(Vector3 position) => _breakerTransform.position = position;       


        
        // Breaker Events
        private void OnNewLineInRange(DiscreteLine line)
        {
            _breakerDrawingHandler.AddLine(line);
            LinesUpdated = true;
        }

        private void OnLineOutOfRange(DiscreteLine line)
        {
            _breakerDrawingHandler.RemoveLine(line);
            LinesUpdated = true;
        }


        // Private Methods
        private async UniTask SeekerMovement()
        {
            _seekerActive = true;
            bool anyLineToFind;
            do
            {
                var bestPosition = _breakerDrawingHandler.FindNextPointToGo(_breakerTransform.position);
                var path = _breakerAreaHandler.MinimumPathTo(_breakerTransform.position, bestPosition);
                LinesUpdated = false;
                anyLineToFind = path.Any();
                await FollowPathUntilUpdate(path);
            }
            while (anyLineToFind);
            _seekerActive = false;
        }     

        private async UniTask FollowPathUntilUpdate(IEnumerable<Vector2> path)
        {
            _animator.SetBool("IsMoving", true);
            CancellationTokenSource rotationCancellationSource;
            foreach (var position in path)
                if (LinesUpdated)
                    break;
                else
                {
                    //await TranslateTo(position).WithCancellation(this.GetCancellationTokenOnDestroy());

                    //Rotate & Translate
                    Vector3 relativeTarget = ((Vector3)position - _breakerTransform.position).normalized;
                    Quaternion toQuaternion = Quaternion.FromToRotation(-_propulsionDirection, relativeTarget);
                    rotationCancellationSource = new CancellationTokenSource();
                    RotateTowards(toQuaternion, rotationCancellationSource.Token).WithCancellation(this.GetCancellationTokenOnDestroy()).Forget();
                    await TranslateTo(position).WithCancellation(this.GetCancellationTokenOnDestroy());
                    rotationCancellationSource.Cancel();
                }
            _breakerTransform.rotation = _breakerIdleRotation;
            _animator.SetBool("IsMoving", false);
        }
    }
}