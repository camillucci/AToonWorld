using Assets.AToonWorld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding.Math
{
    public struct StraightLine
    {
        public StraightLine(Vector2 startingPoint, Vector2 direction)
        {
            (StartingPoint, Direction) = (startingPoint, direction.normalized);
        }

        public static StraightLine FromTwoPoints(Vector2 pointaA, Vector2 pointB)
        {
            var direction = (pointB - pointaA).normalized;
            return new StraightLine(pointaA, direction);
        }


        public Vector2 StartingPoint { get; }
        public Vector2 Direction { get; }


        public static bool TryFindSingleInterception(StraightLine lineA, StraightLine lineB, out Vector2 intercept)
        {
            // A (t s) = b

            var A = Matrix2x2.FromColumns(lineA.Direction, -lineB.Direction);
            var b = lineB.StartingPoint - lineA.StartingPoint;
            if(A.TryInvert(out Matrix2x2 inverse))
            {
                var (t, s) = inverse * b;
           
                intercept = lineA.StartingPoint + t * lineA.Direction;
                var pointS = lineB.StartingPoint + s * lineB.Direction;            
                return true;
            }
            intercept = Vector2.zero;
            return false;
        }
    }
}
