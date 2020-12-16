using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.AToonWorld.Scripts.Utils
{
    public class Test
    {
        private bool _cancelled;
        private UniTask _lastTask = UniTask.CompletedTask;        

        public void CreateNewTask()
        {
            SpawnNewTask().Forget();
        }

        public async UniTask SpawnNewTask()
        {
            await CancelLastTask();
            _lastTask = OperationAsync();
        }

        private async UniTask CancelLastTask()
        {
            _cancelled = true;
            await _lastTask;
        }

        private async UniTask OperationAsync()
        {
            while(!_cancelled)
                await UniTask.Delay(500);
        }
    }
}
