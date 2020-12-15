using Assets.AToonWorld.Scripts.Audio;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Extensions
{
    public static class GameObjectExtensions
    {
        public static UniTask PlaySound(this GameObject @this, SoundEffect soundEffect)
            => AudioManager.PrefabInstance?.PlaySound(soundEffect, @this.transform) ?? UniTask.CompletedTask;


        // UniTask with cancellationToken on destroy
        public static UniTask Delay(MonoBehaviour @this, int millisecondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update)
            => UniTask.Delay(millisecondsDelay, ignoreTimeScale, delayTiming, @this.GetCancellationTokenOnDestroy());

        public static UniTask DelayFrame(MonoBehaviour @this, int delayFrameCount, PlayerLoopTiming timing)
            => UniTask.DelayFrame(delayFrameCount, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitForEndOfFrame(this MonoBehaviour @this)
            => UniTask.WaitForEndOfFrame(@this.GetCancellationTokenOnDestroy());

        public static UniTask WaitForFixedUpdate(this MonoBehaviour @this)
            => UniTask.WaitForFixedUpdate(@this.GetCancellationTokenOnDestroy());

        public static UniTask NextFrame(this MonoBehaviour @this, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.NextFrame(timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask Yield(this MonoBehaviour @this, PlayerLoopTiming timing)
            => UniTask.Yield(timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitUntil(MonoBehaviour @this, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.WaitUntil(predicate, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitWhile(MonoBehaviour @this, Func<bool> predicate, PlayerLoopTiming timing = PlayerLoopTiming.Update)
            => UniTask.WaitWhile(predicate, timing, @this.GetCancellationTokenOnDestroy());

        public static UniTask WaitUntilValueChanged<T, U>(MonoBehaviour @this, T target, Func<T, U> monitorFunction, PlayerLoopTiming timing = PlayerLoopTiming.Update, IEqualityComparer<U> equalityComparer = default) where T : class
            => UniTask.WaitUntilValueChanged(target, monitorFunction, timing, equalityComparer, @this.GetCancellationTokenOnDestroy());

    }
}
