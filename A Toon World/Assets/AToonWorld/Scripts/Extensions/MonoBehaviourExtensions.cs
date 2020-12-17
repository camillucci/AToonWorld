using Assets.AToonWorld.Scripts.Audio;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class MonoBehaviourExtensions
    {
        public static void Invoke(this MonoBehaviour @this, Action callback, float delayInSeconds)
        {
            @this.StartCoroutine(InvokeDelayCoroutine(callback, delayInSeconds));
        }

        public static void InvokeFramwDelayed(this MonoBehaviour @this, Action callback, int frameDelay)
        {
            InvokeFrameDelayedTask(@this, callback, frameDelay).Forget();
        }

        private static IEnumerator InvokeDelayCoroutine(Action callback, float delayInSeconds)
        {   
            yield return new WaitForSeconds(delayInSeconds);
            callback.Invoke();
        }


        private static async UniTaskVoid InvokeFrameDelayedTask(MonoBehaviour monoBehaviour, Action action, int frameDelay)
        {
            await UniTask.DelayFrame(frameDelay, cancellationToken: monoBehaviour.GetCancellationTokenOnDestroy());
            action.Invoke();
        }

        public static UniTask PlaySound(this MonoBehaviour @this, SoundEffect soundEffect)
            => @this.gameObject.PlaySound(soundEffect);



        // UniTask with cancellationToken on destroy
        public static UniTask Delay(this MonoBehaviour @this, int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
            => UniTask.Delay(millisecondsDelay, ignoreTimeScale, delayTiming, @this.GetCancellationTokenOnDestroy());

        public static UniTask DelayFrame(this MonoBehaviour @this, int delayFrameCount, PlayerLoopTiming timing)
            => UniTask.DelayFrame(delayFrameCount, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitForEndOfFrame(this MonoBehaviour @this)
            => UniTask.WaitForEndOfFrame(@this.GetCancellationTokenOnDestroy());

        public static UniTask WaitForFixedUpdate(this MonoBehaviour @this)
            => UniTask.WaitForFixedUpdate(@this.GetCancellationTokenOnDestroy());

        public static UniTask NextFrame(this MonoBehaviour @this, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.NextFrame(timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask Yield(this MonoBehaviour @this, PlayerLoopTiming timing)
            => UniTask.Yield(timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitUntil(this MonoBehaviour @this, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.WaitUntil(predicate, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitWhile(this MonoBehaviour @this, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.WaitWhile(predicate, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitUntilValueChanged<T, U>(this MonoBehaviour @this, T target, Func<T, U> monitorFunction, PlayerLoopTiming timing = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = default) where T : class
            => UniTask.WaitUntilValueChanged(target, monitorFunction, timing, equalityComparer, @this.GetCancellationTokenOnDestroy());
    }
}
