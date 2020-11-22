using Assets.AToonWorld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding.Discrete
{
    public class DiscreteLine
    {
        private List<Vector2> _points;
        
        public DiscreteLine (IEnumerable<Vector2> points)
        {
            if (!points.Any())
                throw new InvalidOperationException("A line should have at least one point");

            _points = points.ToList();
        }

        public Vector2 ClosestPointTo(Vector2 point)
        {
            return _points.WithMinOrDefault(p => Vector2.Distance(p, point));
        }
    }
}
