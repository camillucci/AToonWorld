using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding.Math;
using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding.Utils
{
    public static class CurvePathAnimations
    {
       public static Task CubicBezierFromPoints(Vector2 start, Vector2 middlePoint, Vector2 end, float speed, Action<Vector2> callback)
       {
            Vector2 PositionAtTime(float t)
            {
                var position = middlePoint + (1 - t) * (1 - t) * (start - middlePoint) + t * t*(end - middlePoint);
                return position;
            }

            return Animations.Transition
            (
                from: 0,
                to: 1,
                time => callback.Invoke(PositionAtTime(time)),
                speed: speed,
                smooth: false
            );
       }


        public static Task CubicBezierFromDirections(Vector2 start, Vector2 end, Vector2 startDirection, Vector2 endDirection, float speed, Action<Vector2> positionCallback)
        {
            var lineStart = new StraightLine(start, startDirection);
            var lineEnd = new StraightLine(end, endDirection);
            if (StraightLine.TryFindSingleInterception(lineStart, lineEnd, out Vector2 intersection))
                return CubicBezierFromPoints(start, intersection, end, speed, positionCallback);
            return Task.CompletedTask;
        }      
    }
}
