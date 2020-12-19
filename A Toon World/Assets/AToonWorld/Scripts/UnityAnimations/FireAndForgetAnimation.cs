using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UnityAnimations
{
    public class FireAndForgetAnimation : MonoBehaviour
    {
        // Private fields
        private UniTaskCompletionSource _tcs;


        // Initialization
        private void Awake()
        {
            Animator = GetComponent<Animator>();
            Animator.enabled = false;            
        }


        // Public Properties
        public Animator Animator { get; private set; }


        // Public Methods
        public void PlayAndForget()
        {
            Animator.enabled = true;
        }


        public UniTask Play()
        {
            PlayAndForget();
            _tcs = new UniTaskCompletionSource();
            return _tcs.Task;
        }


        // UnityEvent handlers
        private void NotifyAnimationEnd()
        {
            _tcs?.TrySetResult();
            Destroy(this);
        }
    }
}
