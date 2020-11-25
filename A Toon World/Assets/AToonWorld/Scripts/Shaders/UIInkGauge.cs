using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInkGauge : WavingSprite
{
    [SerializeField] private bool _decreaseAmplitudeWhenFull = true;
    [SerializeField] private float _maxAmplitude = 4;

    private Image _imageComponent;

    void Awake() {
        _imageComponent = GetComponent<Image>();
    }

    public void SetFillAmmount(float fillAmmount)
    {
        if(_decreaseAmplitudeWhenFull)
            _imageMaterial.SetFloat("_Amplitude", _maxAmplitude * (1 - fillAmmount));
        _imageComponent.fillAmount = fillAmmount;
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
