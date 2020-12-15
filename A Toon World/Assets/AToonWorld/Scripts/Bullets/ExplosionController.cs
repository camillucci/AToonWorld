using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Assets.AToonWorld.Scripts.Extensions;

public class ExplosionController : MonoBehaviour
{
    private const int _animationTime = 1000;
    void Start()
    {
        this.Delay(_animationTime).ContinueWith(() => Destroy(gameObject)).Forget();
    }
}
