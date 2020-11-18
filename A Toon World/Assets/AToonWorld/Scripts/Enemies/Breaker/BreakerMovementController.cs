using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using Assets.AToonWorld.Scripts.PathFinding.Utils;
using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{       
    public class BreakerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _turnOffset;


        // Private Fields
        private BreakerBody _breakerBody;
        private BreakerTargetAreaHandler _breakerAreaHandler;
        private Transform _breakerTransform;                
        private GridController _gridController;
        private bool _canFollowPath;
        private Task _followPathTask = Task.CompletedTask;



        // Initialization
        private void Awake()
        {
            _gridController = GetComponent<GridController>();
            _breakerBody = GetComponentInChildren<BreakerBody>();
            _breakerAreaHandler = GetComponentInChildren<BreakerTargetAreaHandler>();
            _breakerTransform = _breakerBody.transform;
            InitializeBreakerAreaCollider();
        }

        private void InitializeBreakerAreaCollider()
        {
            
        }


        // Public Properties
        public INode CurrentNode => _gridController.WorldPointToNode(_breakerTransform.position);



        // Public Methods
        public Task TranslateTo(Vector3 position) => _breakerTransform.MoveToAnimated(position, _speed, false);
        public Task TranslateTo(INode node) => _breakerTransform.MoveToAnimated(_gridController.NodeToWorldPoint(node), _speed, false);
        public void TeleportTo(INode node) => _breakerTransform.position = _gridController.NodeToWorldPoint(node);
        public void TeleportTo(Vector3 position) => _breakerTransform.position = position;
        public async Task TryMoveTo(Vector3 position)
        {            
            var path = _breakerAreaHandler.MinimumPathTo(_breakerTransform.position, position);
            await FollowPath(path);
        }



        // Unity Events
        private void Update()
        {
            TestUpdate();
        }



        // TEST
        private async void TestUpdate()
        {
            if (Input.GetMouseButtonDown(0))
                await TestMoveToMousePosition();
        }

        private async Task TestMoveToMousePosition()
        {
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(screenPosition);           
            await TryMoveTo(worldPosition);
        }



        // Private Methods
        private async Task FollowPath(IList<Vector2> positions)
        {
            await CancelExistingPath();
            async Task FollowPathTask()
            {
                foreach (var position in positions)
                    if (!_canFollowPath)
                        return;
                    else 
                        await TranslateTo(position);
            }

            _followPathTask = FollowPathTask();
        }        


        private async Task CancelExistingPath()
        {
            _canFollowPath = false;
            await _followPathTask;
            _canFollowPath = true;
        }
    }
}