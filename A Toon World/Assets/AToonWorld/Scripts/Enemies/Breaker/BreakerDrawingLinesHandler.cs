using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{
    public class BreakerDrawingHandler
    { 
        // Private Fields
        private List<DiscreteLine> _lines = new List<DiscreteLine>();
        private Vector2 _idlePosition;
        
        
        // Initialization
        public BreakerDrawingHandler(Vector2 idlePosition)
        {
            _idlePosition = idlePosition;
        }



        // Public Properties
        public IReadOnlyList<DiscreteLine> Lines => _lines;



        // Public Methods
        public void AddLine(DiscreteLine line)
        {
            _lines.Add(line);
        }

        public void RemoveLine(DiscreteLine line)
        {
            _lines.Remove(line);
        }

        public Vector2 FindNextPointToGo(Vector2 currentPosition)
        {
            if (!_lines.Any())
                return _idlePosition;

            var closestPoints = from line in _lines select line.ClosestPointTo(currentPosition);            
            return closestPoints.WithMinOrDefault(point => Vector2.Distance(point, currentPosition));
        }
    }
}
