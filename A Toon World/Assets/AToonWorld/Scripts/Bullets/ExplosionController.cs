using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    private const int _animationTime = 1000;
    void Start()
    {
        UniTask.Delay(_animationTime).ContinueWith(() => Destroy(gameObject)).Forget();
    }
}
