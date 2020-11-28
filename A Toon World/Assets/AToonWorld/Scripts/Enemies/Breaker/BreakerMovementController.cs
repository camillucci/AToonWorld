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
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _turnOffset;


        // Private Fields
        public BreakerDrawingHandler _breakerDrawingHandler;
        private BreakerBody _breakerBody;
        private BreakerTargetAreaHandler _breakerAreaHandler;
        private Transform _breakerTransform;                
        private GridController _gridController;
        private bool _canFollowPath;
        private UniTask? _followPathTask = UniTask.CompletedTask;



        // Initialization
        private void Awake()
        {
            _gridController = GetComponent<GridController>();
            _breakerBody = GetComponentInChildren<BreakerBody>();
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
        public UniTask TranslateTo(INode node) => _breakerTransform.MoveToAnimated(_gridController.NodeToWorldPoint(node), _speed, false);
        public void TeleportTo(INode node) => _breakerTransform.position = _gridController.NodeToWorldPoint(node);
        public void TeleportTo(Vector3 position) => _breakerTransform.position = position;
        public async UniTask TryMoveTo(Vector3 position)
        {            
            var path = _breakerAreaHandler.MinimumPathTo(_breakerTransform.position, position);
            await FollowPath(path);
        }


        
        // Breaker Events
        private void OnNewLineInRange(DiscreteLine line)
        {
            _breakerDrawingHandler.AddLine(line);
            FollowBestPath().Forget();
        }

        private void OnLineOutOfRange(DiscreteLine line)
        {
            _breakerDrawingHandler.RemoveLine(line);
            FollowBestPath().Forget();
        }


        // Private Methods
        private async UniTask FollowBestPath()
        {
            var bestPosition = _breakerDrawingHandler.FindNextPointToGo(_breakerTransform.position);
            var path = _breakerAreaHandler.MinimumPathTo(_breakerTransform.position, bestPosition);
            await FollowPath(path);
        }


        private async UniTask FollowPath(IList<Vector2> positions)
        {
            await CancelExistingPath();
            async UniTask FollowPathTask()
            {
                foreach (var position in positions)
                    if (!_canFollowPath)
                        return;
                    else 
                        await TranslateTo(position).WithCancellation(this.GetCancellationTokenOnDestroy());
            }

            _followPathTask = FollowPathTask();
        }        


        private async UniTask CancelExistingPath()
        {
            _canFollowPath = false;
            if(_followPathTask != null)
            {
                var task = _followPathTask.Value;
                _followPathTask = null;
                await task;
            }
            _canFollowPath = true;
        }
    }
}