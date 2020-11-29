using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInkGauge : WavingSprite
{
    [SerializeField] private bool _decreaseAmplitudeWhenFull = true;
    [SerializeField] private float _maxAmplitude = 0.8f;

    protected override void Awake() {
        base.Awake();
    }

    public void SetFillAmount(float fillAmount)
    {
        if(_decreaseAmplitudeWhenFull)
            _imageMaterial.SetFloat("_Amplitude", _maxAmplitude * (1 - fillAmount));
        _imageComponent.fillAmount = fillAmount;
    }

    protected override void DestroyInherited()
    {
        base.DestroyInherited();

        #if UNITY_EDITOR
            //Clean Up
            _imageMaterial.SetFloat("_Amplitude", _maxAmplitude);
        #endif
    }
}
