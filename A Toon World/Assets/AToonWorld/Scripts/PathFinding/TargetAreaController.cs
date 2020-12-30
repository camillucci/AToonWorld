using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using Cysharp.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    [RequireComponent(typeof(GridController))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TargetAreaController : MonoBehaviour
    {
        // Private Fields
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();
        private readonly HashSet<string> _obstaclesTags = new HashSet<string>();
        private readonly HashSet<Collider2D> _obstaclesColliders = new HashSet<Collider2D>();
        protected readonly PathStepsContainer _forbiddenStepsContainer = new PathStepsContainer();
        protected GridController _gridController;
        private BoxCollider2D _boxCollider;
        private bool _hasUpdatedCollisionsThisFrame = false;


        // Initialization
        private void Awake()
        {
            _gridController = GetComponent<GridController>();
            _boxCollider = GetComponent<BoxCollider2D>();
        }

        protected virtual void Start()
        {
            UpdateColliderSize();
        }


        // Public Properties
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;
        public IReadOnlyCollection<Collider2D> NotWalkableColliders => _obstaclesColliders;



        // Public Methods
        public IList<Vector2> MinimumPathTo(Vector2 from, Vector2 to)
        {
            var (startNode, destinationNode) = (_gridController.WorldPointToNode(from), _gridController.WorldPointToNode(to));
            var path = _gridController.Grid.FindMinimumPath(startNode, destinationNode, _forbiddenStepsContainer)
                           .Select(node => _gridController.NodeToWorldPoint(node))
                           .ToList();
            if (path.Any())
            {
                path.Remove(path.Last());
                path.Add(to);
            }
            return path;
        }


        // UnityEvents
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _colliderTrigger.NotifyEnter(collision.gameObject.tag, collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _colliderTrigger.NotifyExit(collision.gameObject.tag, collision);
        }




        // TargetAreaEvents
        private void OnNotWalkableEnter(Collider2D collision)
        {
            _obstaclesColliders.Add(collision);
            UpdateNotWalkableArea();
        }

        private void OnNotWalkableExit(Collider2D collision)
        {
            _obstaclesColliders.Remove(collision);
            UpdateNotWalkableArea();
        }

        // Private Methods
        private void UpdateColliderSize()
        {
            var grid = _gridController.Grid;
            var width = (_gridController.NodeToWorldPoint(grid.TopRight) - _gridController.NodeToWorldPoint(grid.TopLeft)).x + 1;
            var height = (_gridController.NodeToWorldPoint(grid.TopLeft) - _gridController.NodeToWorldPoint(grid.BottomLeft)).y + 1;
            Vector2 colliderSize = _boxCollider.bounds.size;
            _boxCollider.size = new Vector2(_boxCollider.size.x / colliderSize.x * width, _boxCollider.size.y / colliderSize.y * height);
            ForceUpdateColliders();
            UpdateNotWalkableArea();
        }

        private void AddObstacleTag(string tag)
        {
            _obstaclesTags.Add(tag);
            ColliderTrigger.Enter.SubscribeWithTag(tag, OnNotWalkableEnter);
            ColliderTrigger.Exit.SubscribeWithTag(tag, OnNotWalkableExit);
        }

        private void RemoveObstacleTag(string tag)
        {
            _obstaclesTags.Remove(tag);
            ColliderTrigger.Exit.UnSubscribeWithTag(tag, OnNotWalkableEnter);
            ColliderTrigger.Exit.UnSubscribeWithTag(tag, OnNotWalkableExit);
        }



        private void ForceUpdateColliders()
        {
            Vector2 center = _boxCollider.bounds.center;
            Vector2 extends = _boxCollider.bounds.extents;
            var colliders = Physics2D.OverlapBoxAll(center, extends, 0);
            foreach (var collider in NotWalkableColliders)
                OnNotWalkableExit(collider);
            foreach (var collider in colliders)
                _colliderTrigger.NotifyEnter(collider.gameObject.tag, collider);
        }

        private void UpdateNotWalkableArea()
        {
            if(!_hasUpdatedCollisionsThisFrame)
            {
                _hasUpdatedCollisionsThisFrame = true;
                var grid = _gridController.Grid;
                foreach (var node in grid)
                    node.Walkable = IsNodeWalkable(node);
                UpdateForbiddenSteps();

                ExecuteAtEndOfFrame(() => {
                    _hasUpdatedCollisionsThisFrame = false;
                }).Forget();
            }
        }

        private bool IsNodeWalkable(INode node)
        {
            var nodePositions = _gridController.GetNodeBounds(node);
            var collider = Physics2D.OverlapArea(nodePositions.min, nodePositions.max, LayerMask.GetMask(UnityTag.NonWalkable));
            if(collider != null)
                return false;
            return true;
        }


        private void UpdateForbiddenSteps()
        {
            var grid = _gridController.Grid;

            foreach (var node in grid)
                if (!node.Walkable)
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
                    _forbiddenStepsContainer.Add(grid[nodeACoordinates], grid[nodeBCoordinates]);
            }
        }

        private async UniTaskVoid ExecuteAtEndOfFrame(System.Action action)
        {
            await UniTask.WaitForEndOfFrame();
            action.Invoke();
        }
    }
}
