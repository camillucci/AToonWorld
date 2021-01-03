using Assets.AToonWorld.Scripts.Extensions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Utils
{
    public class SingletonTaskManager
    {
        // Private Fields
        private Component _bindedComponent;
        private UniTask _currentTask = UniTask.CompletedTask;



        // Initialization
        public SingletonTaskManager(Component bindedComponent)
        {
            _bindedComponent = bindedComponent;            
        }


        // Public Properties
        public bool IsRunning => _currentTask.Status == UniTaskStatus.Pending;
        public bool IsCancelling { get; private set; }


        // Public Methods
        public void ReplaceTask(UniTask task)
        {
            async UniTaskVoid ReplaceCurrentTask()
            {
                await CancelTask();
                _currentTask = task;
            }
            ReplaceCurrentTask().Forget();
        }

        public async UniTask CancelTask()
        {
            while (IsCancelling)
                await _bindedComponent.NextFrame(); // The only way that does not produce garbage is polling. (i.e. an event that notifies that the cancelation has completed contains a list of delegates...)

            IsCancelling = true;
            await _currentTask;
            _currentTask = UniTask.CompletedTask;
            IsCancelling = false;
        }
    }
}
