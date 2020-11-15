using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
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
        [SerializeField] private float _speed;                

        // Private Fields
        private PathFindingGridController _gridController;
        private BreakerBody _breakerBody;
        private BreakerAreaCollider _breakerAreaCollider;
        private Transform _breakerTransform;
        private readonly IList<string> _nonWalkableTags = new List<string> { UnityTag.Ground };
        private Task _testTask = Task.CompletedTask;


        // Initialization
        private void Awake()
        {
            _gridController = GetComponentInChildren<PathFindingGridController>();
            _breakerBody = GetComponentInChildren<BreakerBody>();
            _breakerAreaCollider = GetComponentInChildren<BreakerAreaCollider>();
            _breakerTransform = _breakerBody.transform;            
        }

        private void Start()
        {
            if (!Application.isPlaying)
                return;

            TeleportTo(_gridController.Grid[0, 0]);                   
        }


        // Public Properties
        public INode CurrentNode => _gridController.WorldPointToNode(_breakerTransform.position);



        // Public Methods
        public Task TranslateTo(Vector3 position) => _breakerTransform.MoveToAnimated(position, _speed, false);
        public Task TranslateTo(INode node) => _breakerTransform.MoveToAnimated(_gridController.NodeToWorldPoint(node), _speed, false);
        public void TeleportTo(INode node) => _breakerTransform.position = _gridController.NodeToWorldPoint(node);
        public void TeleportTo(Vector3 position) => _breakerTransform.position = position;
        public async Task MoveTo(Vector3 position)
        {
            UpdateUnwalkableArea();
            var grid = _gridController.Grid;
            var path = grid.FindMinimumPath(CurrentNode, _gridController.WorldPointToNode(position));
            await FollowPath(path);
        }


        // Unity Events
        private void Update()
        {
            TestUpdate();
        }



        // Breaker Events


        // Private Methods


        private void TestUpdate()
        {
            if (_breakerAreaCollider.NotWalkableCollidersInside.Any() && _testTask.IsCompleted && Input.GetMouseButtonDown(0))
                _testTask = TestMoveToMousePosition();
        }

        private async Task TestMoveToMousePosition()
        {
            if (!_testTask.IsCompleted)
                return;
            Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            Vector2 worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(screenPosition);

            await MoveTo(worldPosition);
        }



        private async Task FollowPath(IEnumerable<INode> path)
        {
            foreach (var node in path)
                await TranslateTo(node);
        }

        private void UpdateUnwalkableArea()
        {
            var colliders = _breakerAreaCollider.NotWalkableCollidersInside;
            foreach (var node in _gridController.Grid)
                node.Walkable = IsNodeWalkable(node, colliders);
        }

        private bool IsNodeWalkable(INode node, IEnumerable<Collider2D> colliders)
        {
            var nodePositions = _gridController.GetNodeBoundsAndCenter(node);
            foreach (var nodePosition in nodePositions)
                foreach (var collider in colliders)
                    if (collider.bounds.Contains(nodePosition))
                        return false;
            return true;
        }
    }
}
