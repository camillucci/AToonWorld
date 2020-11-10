using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class DrawSplineController : MonoBehaviour
{
    //FIXME: La fisica con Rigidbody2D attualmente è rotta sulle linee (probabilmente perchè assume che l'origine dell'oggetto sia il centro di massa?)
    
    #region Components

    private LineRenderer _inkLineRenderer;
    private EdgeCollider2D _splineCollider;
    //private Rigidbody2D _splineRigidBody;

    #endregion

    #region Fields
    
    [SerializeField] private float _minDistanceBetweenPoints = 0.01f;

    #endregion

    private Vector3[] _splinePoints;
    private Transform _splineTransform;
    private int _currentSplineIndex;

    void Awake()
    {
        _inkLineRenderer = GetComponent<LineRenderer>();
        //_splineRigidBody = GetComponent<Rigidbody2D>();
        _splineTransform = GetComponent<Transform>();
        _splineCollider = GetComponent<EdgeCollider2D>();
        this.Clear();
    }

    public void Clear()
    {
        //_splineRigidBody.simulated = false;
        _inkLineRenderer.positionCount = 0;
        _splineCollider.points = null;
        _splineCollider.enabled = false;
        _splineTransform.position = Vector3.zero;
        _splineTransform.rotation = Quaternion.identity;
        _currentSplineIndex = 0;
    }

    public void AddPoint(Vector2 newPoint)
    {
        //First point
        if(_currentSplineIndex == 0)
        {
            _inkLineRenderer.positionCount++;
            _inkLineRenderer.SetPosition(_currentSplineIndex++, newPoint);
            return;
        }

        //Spline points
        Vector2 lastPoint = _inkLineRenderer.GetPosition(_currentSplineIndex - 1);
        if(IsValidPoint(newPoint, lastPoint))
        {
            _inkLineRenderer.positionCount++;
            _inkLineRenderer.SetPosition(_currentSplineIndex++, newPoint);
        }
    }

    public void EnableSimulation()
    {
        //Generate Collider
        _splinePoints = new Vector3[_inkLineRenderer.positionCount];
        _inkLineRenderer.GetPositions(_splinePoints);
        _splineCollider.points = Array.ConvertAll<Vector3, Vector2>(_splinePoints, p => (Vector2)p);
        _splineCollider.enabled = true;

        //TODO: Custom Physics?
        //_splineRigidBody.simulated = true;
    }

    bool IsValidPoint(Vector2 point, Vector2 lastPoint)
    {
        float distance = (point - lastPoint).magnitude;
        return distance > _minDistanceBetweenPoints;
    }
}
