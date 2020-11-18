using Assets.AToonWorld.Scripts.PathFinding;
using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{
    public class BreakerTargetAreaHandler : MonoBehaviour
    {
        // Private Fields
        private readonly TaggedEvent<string, Collider2D> _triggerEnter = new TaggedEvent<string, Collider2D>();
        private readonly TaggedEvent<string, Collider2D> _triggerExit = new TaggedEvent<string, Collider2D>();
        private readonly HashSet<Collider2D> _colliders = new HashSet<Collider2D>();
        private readonly List<string> _notWalkableTags = new List<string> { UnityTag.Ground };
        private readonly PathStepsContainer _fobiddenStepsContainer = new PathStepsContainer();        
        private readonly Dictionary<DrawSplineController, DiscreteLine> _drawingsInRange = new Dictionary<DrawSplineController, DiscreteLine>();
        private BoxCollider2D _boxCollider;
        private GridController _gridController;



        // Initialization
        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _gridController = GetComponent<GridController>();
            InitializeTriggerEnter();
        }

        private void InitializeTriggerEnter()
        {           
            foreach (var tag in _notWalkableTags)
            {
                TriggerEnter.SubscribeWithTag(tag, OnNotWalkableEnter);
                TriggerExit.SubscribeWithTag(tag, OnNotWalkableExit);
            }

            TriggerEnter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
            TriggerExit.SubscribeWithTag(UnityTag.Drawing, OnDrawingExit);
        }


        
        // Events
        public event Action<DiscreteLine> NewLineInRange;
        public event Action<DiscreteLine> LineOutOfRange;



        // Public Properties
        public ITaggedEvent<string, Collider2D> TriggerEnter => _triggerEnter;
        public ITaggedEvent<string, Collider2D> TriggerExit => _triggerExit;
        public IReadOnlyCollection<Collider2D> NotWalkableColliders=> _colliders;





        // Public Methods
        public void SetColliderSize(Vector2 size)
        {
            Vector2 colliderSize = _boxCollider.bounds.size;
            _boxCollider.size = new Vector2(_boxCollider.size.x / colliderSize.x * size.x, _boxCollider.size.y / colliderSize.y * size.y);
        }

        public IList<Vector2> MinimumPathTo(Vector2 from, Vector2 to)
        {
            var (startNode, destinationNode) = (_gridController.WorldPointToNode(from), _gridController.WorldPointToNode(to));                        
            var path = _gridController.Grid.FindMinimumPath(startNode, destinationNode, _fobiddenStepsContainer)
                           .Select(node => _gridController.NodeToWorldPoint(node))
                           .ToList();
            if(path.Any())
            {
                path.Remove(path.Last());
                path.Add(to);
            }
            return path;
        }




        // Unity Events
        private void OnTriggerEnter2D(Collider2D collision)
        {
             _triggerEnter.InvokeWithTag(collision.gameObject.tag, collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _triggerExit.InvokeWithTag(collision.gameObject.tag, collision);
        }





        // Breaker Events
        private void OnNotWalkableEnter(Collider2D collision)
        {            
            _colliders.Add(collision);
            UpdateUnwalkableArea();
        }

        private void OnNotWalkableExit(Collider2D collision)
        {
            _colliders.Remove(collision);
            UpdateUnwalkableArea();
        }

        private void OnDrawingEnter(Collider2D collision)
        {            
            var gameObject = collision.gameObject;
            var spLineController = gameObject.GetComponent<DrawSplineController>();
            AddLine(spLineController);
        }


        private void OnDrawingExit(Collider2D collision)
        {
            var gameObject = collision.gameObject;
            var spLineController = gameObject.GetComponent<DrawSplineController>();
            RemoveLine(spLineController);
        }






        // Private Methods

        private void AddLine(DrawSplineController drawSplineController)
        {
            var linePoints = from point in drawSplineController.SpLinePoints
                             where _gridController.IsInsideGrid(point)
                             where _gridController.WorldPointToNode(point).Walkable
                             select new Vector2(point.x, point.y);
            if (!linePoints.Any())
                return;
            var discreteLine = new DiscreteLine(linePoints);
            _drawingsInRange.Add(drawSplineController, discreteLine);            
            NewLineInRange?.Invoke(discreteLine);
        }

        private void RemoveLine(DrawSplineController drawSplineController)
        {
            if(_drawingsInRange.TryGetValue(drawSplineController, out var discreteLine))
            {
                _drawingsInRange.Remove(drawSplineController);           
                LineOutOfRange?.Invoke(discreteLine);
            }
        }


        private void ForceUpdateColliders()
        {            
            Vector2 center = _boxCollider.bounds.center;
            Vector2 extends = _boxCollider.bounds.extents;
            var colliders = Physics2D.OverlapBoxAll(center, extends, 0);
            foreach (var collider in _colliders)
                OnNotWalkableExit(collider);
            foreach (var collider in colliders)
                _triggerEnter.InvokeWithTag(collider.gameObject.tag, collider);
        }

        private void UpdateUnwalkableArea()
        {
            var grid = _gridController.Grid;
            foreach (var node in grid)
                node.Walkable = IsNodeWalkable(node);
            UpdateForbiddenSteps();
        }

        private bool IsNodeWalkable(INode node)
        {
            var nodePositions = _gridController.GetNodeBoundsAndCenter(node);
            foreach (var nodePosition in nodePositions)
                foreach (var collider in NotWalkableColliders)
                    if (collider.bounds.Contains(nodePosition))
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
                    _fobiddenStepsContainer.Add(grid[nodeACoordinates], grid[nodeBCoordinates]);
            }
        }
    }
}