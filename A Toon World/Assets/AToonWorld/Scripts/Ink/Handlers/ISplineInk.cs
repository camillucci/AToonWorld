using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface which describes the behaviour of Spline based Inks
/// </summary>
public interface ISplineInk
{
    /// <summary>
    /// Called once the spline is enabled
    /// </summary>
    /// <param name="splineController">Assigned spline to the current drawing</param>
    void BindSpline(DrawSplineController splineController);
}
