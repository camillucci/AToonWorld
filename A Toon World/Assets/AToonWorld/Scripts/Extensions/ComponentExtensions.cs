using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class ComponentExtensions
    {
        // UniTask with cancellationToken on destroy
        public static UniTask Delay(this Component @this, int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
            => UniTask.Delay(millisecondsDelay, ignoreTimeScale, delayTiming, @this.GetCancellationTokenOnDestroy());

        public static UniTask DelayFrame(this Component @this, int delayFrameCount, PlayerLoopTiming timing)
            => UniTask.DelayFrame(delayFrameCount, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitForEndOfFrame(this Component @this)
            => UniTask.WaitForEndOfFrame(@this.GetCancellationTokenOnDestroy());

        public static UniTask WaitForFixedUpdate(this Component @this)
            => UniTask.WaitForFixedUpdate(@this.GetCancellationTokenOnDestroy());

        public static UniTask NextFrame(this Component @this, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.NextFrame(timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask Yield(this Component @this, PlayerLoopTiming timing)
            => UniTask.Yield(timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitUntil(this Component @this, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.WaitUntil(predicate, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitWhile(this Component @this, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.WaitWhile(predicate, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitUntilValueChanged<T, U>(this Component @this, T target, Func<T, U> monitorFunction, PlayerLoopTiming timing = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = default) where T : class
            => UniTask.WaitUntilValueChanged(target, monitorFunction, timing, equalityComparer, @this.GetCancellationTokenOnDestroy());




        // Animations with UniTask
        private const float _epsilon = 0.006f;

        public static async UniTask Transition(this Component @this, float from, float to, Action<float> callback, float speed, bool smooth, Func<bool> cancelCondition)
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
                await @this.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(this Component @this, float from, float to, Action<float> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
            => Transition(@this, from, to, callback, speed, smooth, () => token.IsCancellationRequested);



        public static async UniTask Transition(this Component @this, Vector2 from, Vector2 to, Action<Vector2> callback, float speed, bool smooth, Func<bool> cancelCondition)
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
                await @this.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(this Component @this, Vector2 from, Vector2 to, Action<Vector2> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
          => Transition(@this, from, to, callback, speed, smooth, () => token.IsCancellationRequested);




        public static async UniTask Transition(this Component @this, Vector3 from, Vector3 to, Action<Vector3> callback, float speed, bool smooth, Func<bool> cancelCondition)
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
                await @this.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(this Component @this, Vector3 from, Vector3 to, Action<Vector3> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
            => Transition(@this, from, to, callback, speed, smooth, () => token.IsCancellationRequested);

        public static async UniTask Transition(this Component @this, Quaternion from, Quaternion to, Action<Quaternion> callback, float speed, bool smooth, Func<bool> cancelCondition)
        {
            var current = from;
            bool CheckIsCancelled() => cancelCondition?.Invoke() ?? false;
            bool isCancelled = CheckIsCancelled();
            while (Quaternion.Angle(current, to) > _epsilon && !isCancelled)
            {
                var deltaPos = speed * Time.deltaTime;
                current = smooth
                    ? Quaternion.Slerp(current, to, deltaPos)
                    : Quaternion.RotateTowards(current, to, deltaPos);

                callback.Invoke(current);
                await @this.NextFrame();
                isCancelled = CheckIsCancelled();
            }

            if (!isCancelled)
                callback.Invoke(to);
        }

        public static UniTask Transition(this Component @this, Quaternion from, Quaternion to, Action<Quaternion> callback, float speed = 10f, bool smooth = true, CancellationToken token = default)
            => Transition(@this, from, to, callback, speed, smooth, () => token.IsCancellationRequested);
    }
}
