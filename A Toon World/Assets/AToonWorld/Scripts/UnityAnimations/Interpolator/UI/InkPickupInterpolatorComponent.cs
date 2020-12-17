using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;

public class InkPickupInterpolatorComponent : Interpolator
{
    private InkSelectorController _inkSelector;
    [SerializeField] private Transform _boundPickup;

    private void Start()
    {
        _inkSelector = InGameUIController.PrefabInstance.InkSelector;
        
        _interpolationPoints[0] = new InterpolationPoint(_interpolationPoints[0]) {
            transform = _boundPickup
        };

        _interpolationPoints[_interpolationPoints.Count - 1] = new InterpolationPoint(_interpolationPoints[_interpolationPoints.Count - 1]) {
            dynamicPoint = _inkSelector.GetDynamicPointConverter(PlayerInkController.InkType.Construction)
        };
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        _currentTransform.position = _boundPickup.position;
    }
}
