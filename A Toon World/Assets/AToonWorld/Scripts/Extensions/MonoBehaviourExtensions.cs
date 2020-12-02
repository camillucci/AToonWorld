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

        private static IEnumerator InvokeDelayCoroutine(Action callback, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            callback.Invoke();
        }

        public static UniTask PlaySound(this MonoBehaviour @this, string soundName)
            => AudioManager.Instance.PlaySound(soundName, @this.transform);
    }
}
