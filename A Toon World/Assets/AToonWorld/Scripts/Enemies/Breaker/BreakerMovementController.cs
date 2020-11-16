using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using Assets.AToonWorld.Scripts.PathFinding.Coordinates;
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
        private PathFindingGridController _gridController;
        private BreakerBody _breakerBody;
        private BreakerAreaCollider _breakerAreaCollider;
        private Transform _breakerTransform;
        private readonly PathStepsContainer _fobiddenStepsContainer = new PathStepsContainer();
        private Task _testTask = Task.CompletedTask;        


        // Initialization
        private void Awake()
        {
            _gridController = GetComponentInChildren<PathFindingGridController>();
            _breakerBody = GetComponentInChildren<BreakerBody>();
            _breakerAreaCollider = GetComponentInChildren<BreakerAreaCollider>();
            _breakerTransform = _breakerBody.transform;            
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
            var targetPosition = _gridController.WorldPointToNode(position);
            var path = grid.FindMinimumPath(CurrentNode, targetPosition, _fobiddenStepsContainer);
            await FollowPath(path, position);
        }


        // Unity Events
        private void Update()
        {
            TestUpdate();
        }



        // TEST
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
 



        // Private Methods
        private void UpdateUnwalkableArea()
        {
            var colliders = _breakerAreaCollider.NotWalkableCollidersInside;
            foreach (var node in _gridController.Grid)
                node.Walkable = IsNodeWalkable(node, colliders);
            UpdateForbiddenSteps();
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

        private async Task FollowPath(IEnumerable<INode> path, Vector3 finalDestination)
        {
            if (!path.Any())
                return;
            var positions = path.Select(node => _gridController.NodeToWorldPoint(node)).ToList();
            positions.Remove(positions.Last());
            positions.Add(finalDestination);

            foreach (var position in positions)
                await TranslateTo(position);
        }   


        private void UpdateForbiddenSteps()
        {
            var grid = _gridController.Grid;

            foreach (var node in grid)
                if(!node.Walkable)
                {
                    var left = (node.X - 1, node.Y);
                    var top = (node.X, node.Y + 1);
                    var right = (node.X + 1, node.Y);
                    var bottom = (node.X, node.Y - 1);
                    AddIfIsForbiddenStep(left, top);
                    AddIfIsForbiddenStep(top, right);
                    AddIfIsForbiddenStep(right, bottom);
                    AddIfIsForbiddenStep(bottom, left);
                }


            bool IsForbiddenStep((int, int) nodeACoordinates, (int, int) nodeBCoordinates)
            {
                var (xA, yA) = nodeACoordinates;
                var (xB, yB) = nodeBCoordinates;
                if (grid.TryGetValue(xA, yA, out INode nodeA))
                    if (grid.TryGetValue(xB, yB, out INode nodeB))
                        return nodeA.Walkable && nodeB.Walkable;
                return false;
            }
            
            void AddIfIsForbiddenStep((int, int) nodeACoordinates, (int, int) nodeBCoordinates)
            {
                if (IsForbiddenStep(nodeACoordinates, nodeBCoordinates))
                    _fobiddenStepsContainer.Add(grid[nodeACoordinates], grid[nodeBCoordinates]);
            }
        }
    }
}
;