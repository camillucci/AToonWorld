using Assets.AToonWorld.Scripts.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingFading : MonoBehaviour
{
    [SerializeField] private int _startDelay = 0;
    [SerializeField] private int _timeActiveSeconds = 0;
    [SerializeField] private int _timeInactiveSeconds = 0;


    // Private Fields
    private Animator _animator;



    // Initialization
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        this.Invoke(() => StartFading = true, _startDelay);
    }



    // Public Properties
    public bool FadeIn
    {
        get => _animator.GetBool();
        set => _animator.SetProperty(value);
    }

    public bool StartFading
    {
        get => _animator.GetBool();
        set => _animator.SetProperty(value);
    }



    // Animator event handlers
    private void OnFadeOutExit() => this.Invoke(() => FadeIn = true, _timeInactiveSeconds);
    private void OnFadeInExit() => this.Invoke(() => FadeIn = false, _timeActiveSeconds);
}
