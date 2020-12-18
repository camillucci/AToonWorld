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

        Vector3 startPoint = boundPickup.position;
        Vector3 endPoint = _boundUIElement.GetDynamicPoint();
        Vector2 centerPoint = (startPoint + endPoint) / 2;
        Vector2 animationDirection = ((Vector2)(endPoint - startPoint)).normalized;
        Vector2 orthogonalDirection = new Vector2(-animationDirection.y, animationDirection.x);


        _midInterpolationPoints[0] = new InterpolationPoint(_midInterpolationPoints[0]) {
            point = _boundPickup.position + (Vector3)(orthogonalDirection * (Random.Range(0,10) - 5))
        };
    }
}
