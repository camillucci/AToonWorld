using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using UnityEngine;

public class WorldToUIInterpolatorComponent : BezierInterpolator
{
    private ScreenToWorldPointComponent _boundUIElement;
    private Transform _boundPickup;

    public void Bind(ScreenToWorldPointComponent boundUIElement, Transform boundPickup)
    {
        _boundUIElement = boundUIElement;
        _boundPickup = boundPickup;

        _currentTransform.position = _boundPickup.position;

        _interpolationPoints[0] = new InterpolationPoint(_interpolationPoints[0]) {
            transform = _boundPickup
        };

        _interpolationPoints[_interpolationPoints.Count - 1] = new InterpolationPoint(_interpolationPoints[_interpolationPoints.Count - 1]) {
            dynamicPoint = _boundUIElement
        };

        _midInterpolationPoints[0] = new InterpolationPoint(_midInterpolationPoints[0]) {
            point = _boundPickup.position + (Vector3)(Vector2.up * Random.Range(0,7) + Vector2.right * (Random.Range(0, 20) - 10))
        };
    }
}
