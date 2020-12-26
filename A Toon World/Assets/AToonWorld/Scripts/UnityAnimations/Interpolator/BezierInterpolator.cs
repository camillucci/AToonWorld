using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierInterpolator : Interpolator
{
    /// <summary>
    /// List of interpolation points, needs to be ordered by interpolationLocation
    /// </summary>
    [SerializeField] protected List<InterpolationPoint> _midInterpolationPoints;

    #if UNITY_EDITOR

    protected override void OnValidate() 
    {
        base.OnValidate();

        //At least _interpolationPoints.Count - 1 midPoints
        if(_midInterpolationPoints == null)
            _midInterpolationPoints = new List<InterpolationPoint>(_interpolationPoints.Count - 1);

        while(_midInterpolationPoints.Count < _interpolationPoints.Count - 1)
            _midInterpolationPoints.Add(new InterpolationPoint());
    }

    #endif

    protected override void Interpolate(Vector3 startPoint, Vector3 endPoint, float t)
    {
        if(_currentSelectedPoint > 0)
        {
            Vector3 midPoint = GetInterpolationPoint(_midInterpolationPoints[_currentSelectedPoint - 1]);

            //Quadratic formula
            //B(t) = (1-t)^2 P0 + 2(1-t)t P1 + t^2 P2
            float minusT = 1 - t;
            _currentTransform.position = minusT*minusT*startPoint + 2*minusT*t*midPoint + t*t*endPoint;
        }
    }
}
