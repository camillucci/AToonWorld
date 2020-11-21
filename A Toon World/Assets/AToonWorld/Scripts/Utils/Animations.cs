using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityAsync;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Utils
{
    public static class Animations
    {
        private const float _epsilon = 0.006f;

        public static async Task Transition(float from, float to, Action<float> callback, float speed , bool smooth , int frameSensitivity, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Math.Abs(current - to) > _epsilon && !isCancelled)
            {
                var deltaTime = Time.deltaTime;
                current = smooth
                    ? Mathf.Lerp(current, to, deltaTime * speed)
                    : Mathf.MoveTowards(current, to, speed * Time.deltaTime);

                callback.Invoke(current);
                await new WaitForFrames(frameSensitivity);
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static Task Transition(float from, float to, Action<float> callback, float speed = 10f, bool smooth = true, int frameSensitivity = 1, CancellationToken token = default)
            => Transition(from, to, callback, speed, smooth, frameSensitivity, () => token.IsCancellationRequested);



        public static async Task Transition(Vector2 from, Vector2 to, Action<Vector2> callback, float speed, bool smooth, int frameSensitivity, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Vector2.Distance(current, to) > _epsilon && !isCancelled)
            {
                var deltaTime = Time.deltaTime;
                current = smooth
                    ? Vector2.Lerp(current, to, deltaTime * speed)
                    : Vector2.MoveTowards(current, to, speed * Time.deltaTime);

                callback.Invoke(current);
                await new WaitForFrames(frameSensitivity);
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static Task Transition(Vector2 from, Vector2 to, Action<Vector2> callback, float speed = 10f, bool smooth = true, int frameSensitivity = 1, CancellationToken token = default)
          => Transition(from, to, callback, speed, smooth, frameSensitivity, () => token.IsCancellationRequested);




        public static async Task Transition(Vector3 from, Vector3 to, Action<Vector3> callback, float speed, bool smooth, int frameSensitivity, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Vector3.Distance(current, to) > _epsilon && !isCancelled)
            {
                var deltaTime = Time.deltaTime;
                current = smooth
                    ? Vector3.Lerp(current, to, deltaTime * speed)
                    : Vector3.MoveTowards(current, to, speed * Time.deltaTime);

                callback.Invoke(current);
                await new WaitForFrames(frameSensitivity);
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static Task Transition(Vector3 from, Vector3 to, Action<Vector3> callback, float speed = 10f, bool smooth = true, int frameSensitivity = 1, CancellationToken token = default)
            => Transition(from, to, callback, speed, smooth, frameSensitivity, () => token.IsCancellationRequested);              
    }   
}
