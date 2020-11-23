using Assets.AToonWorld.Scripts.PathFinding.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class VectorExtensions
    {
        public static void Deconstruct(this Vector2 vector, out float x, out float y)
            => (x, y) = (vector.x, vector.y);

        public static PolarVector2D ToPolarCoordinates(this Vector2 @this)
        {
            var (x, y) = (@this.x, @this.y);
            var r = @this.magnitude;

            if(x==0)
                return new PolarVector2D(r, 0.5f * Mathf.PI * (y>  0 ? 1: -1));

            var atan = Mathf.Atan(y / x);
            float theta = 0;
            if (x > 0)
                theta = atan;
            else if (x < 0 && y >= 0)
                theta = atan + Mathf.PI;
            else if (x < 0 && y < 0)
                theta = atan - Mathf.PI;

            return new PolarVector2D(r, theta);
        }

        public static bool Parallel(this Vector2 v1, Vector2 v2)
            => Vector2.Distance(v1.normalized, v2.normalized) < Mathf.Epsilon;
    }
}
