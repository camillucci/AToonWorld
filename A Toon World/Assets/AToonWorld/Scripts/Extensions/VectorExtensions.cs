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
    }
}
