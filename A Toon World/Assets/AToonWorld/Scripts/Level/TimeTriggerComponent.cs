using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class TimeTriggerComponent : MonoBehaviour
{
    [SerializeField] private float _secondsToTrigger = 30;
    [SerializeField] private UnityEvent _secondsPassed = null;

    private CancellationTokenSource _cancellationTokenSource;

    private void Awake() 
    {
        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        TriggerEventDelayed().Forget();
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        _cancellationTokenSource.Cancel();
    }

    private async UniTaskVoid TriggerEventDelayed()
    {
        await UniTask.Delay((int)(_secondsToTrigger * 1000), false, PlayerLoopTiming.Update, _cancellationTokenSource.Token);
        _secondsPassed?.Invoke();
    }
}
