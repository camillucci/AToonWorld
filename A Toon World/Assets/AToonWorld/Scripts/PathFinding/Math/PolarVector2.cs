using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding.Math
{
    public struct PolarVector2D
    {
        public float Theta { get; }
        public float R { get; set; }

        public PolarVector2D(float r, float theta) => (R, Theta) = (r, theta);

        public Vector2 ToVector2() => R * new Vector2(Mathf.Cos(Theta), Mathf.Sin(Theta));
    }
}
