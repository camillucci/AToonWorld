using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(EdgeCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class DrawTrailController : DrawSplineController
{
    
    #region Fields
    
    [SerializeField] private int _trailLength = 30;
    [SerializeField] private bool _canHavePhysics = false;

    #endregion

    public override void AddPoint(Vector2 newPoint)
    {
        if(_inkLineRenderer.positionCount < _trailLength)
            base.AddPoint(newPoint);
        else
        {
            for(int i = 0; i < _inkLineRenderer.positionCount - 1; i++)
                _inkLineRenderer.SetPosition(i, _inkLineRenderer.GetPosition(i + 1));
            _inkLineRenderer.SetPosition(_inkLineRenderer.positionCount - 1, newPoint);
        }
    }

    public override void EnableSimulation()
    {
        if(_canHavePhysics)
            base.EnableSimulation();
    }
}
