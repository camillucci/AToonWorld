using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Utils
{
    public static class Animations
    {
        private const float _epsilon = 0.006f;

        public static async UniTask Transition(float from, float to, Action<float> callback, float speed , bool smooth, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Math.Abs(current - to) > _epsilon && !isCancelled)
            {
                var deltaPos = speed * Time.deltaTime;
                current = smooth
                    ? Mathf.Lerp(current, to, deltaPos)
                    : Mathf.MoveTowards(current, to, deltaPos);

                callback.Invoke(current);
                await UniTask.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(float from, float to, Action<float> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
            => Transition(from, to, callback, speed, smooth, () => token.IsCancellationRequested);



        public static async UniTask Transition(Vector2 from, Vector2 to, Action<Vector2> callback, float speed, bool smooth, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Vector2.Distance(current, to) > _epsilon && !isCancelled)
            {
                var deltaPos = speed * Time.deltaTime;
                current = smooth
                    ? Vector2.Lerp(current, to, deltaPos)
                    : Vector2.MoveTowards(current, to, deltaPos);

                callback.Invoke(current);
                await UniTask.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(Vector2 from, Vector2 to, Action<Vector2> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
          => Transition(from, to, callback, speed, smooth, () => token.IsCancellationRequested);




        public static async UniTask Transition(Vector3 from, Vector3 to, Action<Vector3> callback, float speed, bool smooth, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Vector3.Distance(current, to) > _epsilon && !isCancelled)
            {
                var deltaPos = speed * Time.deltaTime;
                current = smooth
                    ? Vector3.Lerp(current, to, deltaPos)
                    : Vector3.MoveTowards(current, to, deltaPos);

                callback.Invoke(current);
                await UniTask.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(Vector3 from, Vector3 to, Action<Vector3> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
            => Transition(from, to, callback, speed, smooth, () => token.IsCancellationRequested);

        public static async UniTask Transition(Quaternion from, Vector3 direction, Action<Quaternion> callback, float speed, bool smooth, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            float rot_z = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion toRotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            while (Quaternion.Angle(current, toRotation) > _epsilon && !isCancelled)
            {
                var deltaPos = speed * Time.deltaTime;
                current = smooth
                    ? Quaternion.Lerp(current, toRotation, deltaPos)
                    : Quaternion.RotateTowards(current, toRotation, deltaPos);

                callback.Invoke(current);
                await UniTask.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(toRotation);
        }

        public static UniTask Transition(Quaternion from, Vector3 direction, Action<Quaternion> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
            => Transition(from, direction, callback, speed, smooth, () => token.IsCancellationRequested);
    }   
}
