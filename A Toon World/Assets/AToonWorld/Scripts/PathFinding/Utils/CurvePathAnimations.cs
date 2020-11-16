using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding.Coordinates;
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
        public static async Task TransitionCurved(Vector2 fromVersor, Vector2 toVersor, float speed, Action<Vector2> callback, bool clockWise = true)
        {
            var (from, to) = (fromVersor.normalized.ToPolarCoordinates(), toVersor.normalized.ToPolarCoordinates());
            to = new PolarVector2D(to.R, FindMinDistanceAngle(from.Theta, to.Theta));
            await Animations.Transition
            (
                from: from.Theta,
                to: to.Theta,
                callback: theta => callback.Invoke(new PolarVector2D(1, theta).ToVector2()),
                speed: speed,
                smooth: false
            );
        }

        private static float FindMinDistanceAngle(float from, float to)
        {
            var distance = Mathf.Abs(from - to);
            if (!FindNearestTo(to, 1))
                FindNearestTo(to, -1);
            return to;

            bool FindNearestTo(float newTo, int signTwoPi)
            {
                bool changed = false;
                bool isBetter;
                do
                {
                    var tmpTo = newTo + signTwoPi * 2 * Mathf.PI;
                    var newDistance = Mathf.Abs(tmpTo - from);
                    isBetter = newDistance < distance;
                    if (isBetter)
                    {
                        to = tmpTo;
                        distance = newDistance;
                        changed = true;
                    }
                } while (isBetter);
                return changed;
            }
        }
    }
}
