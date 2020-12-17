﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interpolator : MonoBehaviour
{
    public enum InterpolationMode
    {
        Lerp,
        Slerp
    }

    [Serializable]
    public struct InterpolationPoint
    {
        public InterpolationPoint(InterpolationPoint other)
        {
            point = other.point;
            transform = other.transform;
            dynamicPoint = other.dynamicPoint;
            interpolationLocation = other.interpolationLocation;
            interpolationMode = other.interpolationMode;
        }

        [SerializeField] public Vector3 point; //Lowest priority
        [SerializeField] public Transform transform;
        [SerializeField] public InterpolatorDynamicPointComponent dynamicPoint; //Highest Priority
        [SerializeField] [Range(0,1)] public float interpolationLocation;
        [SerializeField] public InterpolationMode interpolationMode;
    }

    /// <summary>
    /// List of interpolation points, needs to be ordered by interpolationLocation
    /// </summary>
    [SerializeField] protected List<InterpolationPoint> _interpolationPoints;

    /// <summary>
    /// Ammount of interpolation
    /// </summary>
    [SerializeField] [Range(0,1)] private float _ammount = 0.0f;
    [SerializeField] private bool _disableWhenReached = true;

    private int _currentSelectedPoint = 0;
    private float _currentDistance = 0;
    protected Transform _currentTransform;

    #if UNITY_EDITOR

    private void OnValidate() 
    {
        //Can't have a null interpolation points list
        if(_interpolationPoints == null)
            _interpolationPoints = new List<InterpolationPoint>(2);

        //At least 2 points
        while(_interpolationPoints.Count < 2)
            _interpolationPoints.Add(new InterpolationPoint());

        //First and last need to be 0 and 1
        if(_interpolationPoints[0].interpolationLocation != 0)
        {
            _interpolationPoints[0] = new InterpolationPoint(_interpolationPoints[0])
            {
                interpolationLocation = 0,
            };
        }

        if(_interpolationPoints[_interpolationPoints.Count - 1].interpolationLocation != 1)
        {
            _interpolationPoints[_interpolationPoints.Count - 1] = new InterpolationPoint(_interpolationPoints[_interpolationPoints.Count - 1])
            {
                interpolationLocation = 1,
            };
        }

        //Elements need to be ordered on interpolationLocation
        for(int i = 1; i < _interpolationPoints.Count - 1; i++)
        {
            if(_interpolationPoints[i].interpolationLocation < _interpolationPoints[i-1].interpolationLocation)
            {
                _interpolationPoints[i] = new InterpolationPoint(_interpolationPoints[i])
                {
                    interpolationLocation = _interpolationPoints[i-1].interpolationLocation + float.Epsilon,
                };
            }
        }
    }

    #endif

    //Useless for now
    private int GetCurrentInterpolationPoint() => _interpolationPoints.BinarySearch(
        new InterpolationPoint(){ interpolationLocation = _ammount},
        Comparer<InterpolationPoint>.Create((x,y) => 
            x.interpolationLocation > y.interpolationLocation ? 1 :
            x.interpolationLocation < y.interpolationLocation ? -1 : 0
    ));

    private Vector3 GetInterpolationPoint(InterpolationPoint interpolationPoint)
    {
        return interpolationPoint.dynamicPoint?.GetDynamicPoint() ??
                interpolationPoint.transform?.position ??
                interpolationPoint.point;
    }

    protected virtual void Awake() 
    {
        _currentTransform = this.transform;
    }

    protected virtual void OnEnable() 
    {
        _currentSelectedPoint = 0;
        _ammount = 0;
        _currentDistance = 0;
    }

    //We could do binary search but we assume this will mostly be driven by animations, so the best case is O(1) for linear search
    //O(n) for the frame where the value jumps too much (very rare situation)
    void Update()
    {
        if(Mathf.Approximately(_ammount,1)) //Maybe too much penalty when not _disableWhenReached
        {
            if(_disableWhenReached)
                this.gameObject.SetActive(false);
            return; 
        }

        if(_ammount >= _interpolationPoints[_currentSelectedPoint].interpolationLocation)
        {
            _currentSelectedPoint++;
            _currentDistance = _interpolationPoints[_currentSelectedPoint].interpolationLocation - _interpolationPoints[_currentSelectedPoint-1].interpolationLocation;
        }

        float segmentInterpolationAmmount = (_ammount - _interpolationPoints[_currentSelectedPoint-1].interpolationLocation) / _currentDistance;
        Vector3 startPoint = GetInterpolationPoint(_interpolationPoints[_currentSelectedPoint-1]);
        Vector3 endPoint = GetInterpolationPoint(_interpolationPoints[_currentSelectedPoint]);
        
        
        switch(_interpolationPoints[_currentSelectedPoint].interpolationMode)
        {
            case InterpolationMode.Lerp:
                _currentTransform.position = Vector3.Lerp(startPoint, endPoint, segmentInterpolationAmmount);
            break;
            case InterpolationMode.Slerp:
                _currentTransform.position = Vector3.Slerp(startPoint, endPoint, segmentInterpolationAmmount);
            break;
        }
    }
}