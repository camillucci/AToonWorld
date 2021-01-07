using Assets.AToonWorld.Scripts.Audio;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        public static void InvokeFrameDelayed(this MonoBehaviour @this, Action callback, int frameDelay,  CancellationToken? cancelToken = null)
        {
            InvokeFrameDelayedTask(@this, callback, frameDelay, cancelToken).Forget();
        }

        private static IEnumerator InvokeDelayCoroutine(Action callback, float delayInSeconds)
        {   
            yield return new WaitForSeconds(delayInSeconds);
            callback.Invoke();
        }


        private static async UniTaskVoid InvokeFrameDelayedTask(MonoBehaviour monoBehaviour, Action action, int frameDelay, CancellationToken? cancelToken = null)
        {
            await UniTask.DelayFrame(frameDelay, cancellationToken: cancelToken.HasValue ? cancelToken.Value : monoBehaviour.GetCancellationTokenOnDestroy());
            if(!cancelToken.HasValue || !cancelToken.Value.IsCancellationRequested)
                action.Invoke();
        }

        public static UniTask PlaySound(this MonoBehaviour @this, SoundEffect soundEffect)
            => @this.gameObject.PlaySound(soundEffect);
    }
}
