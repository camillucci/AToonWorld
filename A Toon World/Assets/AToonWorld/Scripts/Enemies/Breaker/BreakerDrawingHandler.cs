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
        private List<DiscreteLine> _lines = new List<DiscreteLine>();
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
            return _lines.Min(line => line.ClosestTo(currentPosition));
        }
    }
}
