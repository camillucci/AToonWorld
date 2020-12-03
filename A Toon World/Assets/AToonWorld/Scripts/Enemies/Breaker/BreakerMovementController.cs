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
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{       
    public class BreakerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _rotationSpeed = 500f;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _turnOffset;


        // Private Fields
        public BreakerDrawingHandler _breakerDrawingHandler;
        private BreakerBody _breakerBody;
        private BreakerTargetAreaHandler _breakerAreaHandler;
        private Transform _breakerTransform;                
        private GridController _gridController;
        private Animator _animator;
        private UniTask? _followPathTask = UniTask.CompletedTask;
        private bool _seakerActive;

        public BreakerMovementController(bool seakerActive)
        {
            _seakerActive = seakerActive;
        }

        private bool _linesUpdated;

        // Private Properties
        private bool LinesUpdated 
        { 
            get => _linesUpdated;  
            set
            {
                _linesUpdated = value;
                if (value && !_seakerActive)
                    SeakerMovement().WithCancellation(this.GetCancellationTokenOnDestroy()).Forget();
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
            _breakerDrawingHandler = new BreakerDrawingHandler(_breakerTransform.position);
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
        public UniTask RotateTowards2D(Vector2 direction) => _breakerTransform.RotateTowardsAnimated(direction, _rotationSpeed, false);
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
        private async UniTask SeakerMovement()
        {
            _seakerActive = true;
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
            _seakerActive = false;
        }     

        private async UniTask FollowPathUntilUpdate(IEnumerable<Vector2> path)
        {
            _animator.SetBool("IsMoving", true);
            foreach (var position in path)
                if (LinesUpdated)
                    return;
                else
                {
                    await TranslateTo(position).WithCancellation(this.GetCancellationTokenOnDestroy());
                    //Vector2 lookAtDirectionLocal = _breakerTransform.InverseTransformDirection(position - (Vector2)_breakerTransform.position).normalized;
                    //Rotate & Translate
                    //await UniTask.WhenAll(TranslateTo(position).WithCancellation(this.GetCancellationTokenOnDestroy()),
                    //                      RotateTowards2D(lookAtDirectionLocal).WithCancellation(this.GetCancellationTokenOnDestroy()));
                }
            _animator.SetBool("IsMoving", false);
        }
    }
}