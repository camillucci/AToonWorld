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
    public class BreakerTargetAreaHandler : TargetAreaController
    {
        // Private Fields       
        private readonly Dictionary<DrawSplineController, DiscreteLine> _drawingsInRange = new Dictionary<DrawSplineController, DiscreteLine>();


        protected override void Start()
        {
            base.Start();
            InitializeObstacles();
            InitializeDrawings();
        }

        private void InitializeObstacles()
        {
            var obstaclesTags = new string[] { UnityTag.Ground };
            foreach (var tag in obstaclesTags)
                AddStaticObstacle(tag);
        }

        private void InitializeDrawings()
        {
            TriggerEnter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
            TriggerExit.SubscribeWithTag(UnityTag.Drawing, OnDrawingExit);
        }



        
        // Events
        public event Action<DiscreteLine> NewLineInRange;
        public event Action<DiscreteLine> LineOutOfRange;



        // Breaker Events     
        private int _drawingsCounter = 0;
        private void OnDrawingEnter(Collider2D collision)
        {
            print(++_drawingsCounter);
            var gameObject = collision.gameObject;
            var spLineController = gameObject.GetComponent<DrawSplineController>();
            AddLine(spLineController);
        }


        private void OnDrawingExit(Collider2D collision)
        {
            print(--_drawingsCounter);
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
    }
}