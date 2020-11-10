using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInkHandler
{
    void OnDrawDown(Vector2 mouseWorldPosition, DrawSplineController splineController);
    void OnDrawHeld(Vector2 mouseWorldPosition, DrawSplineController splineController);
    void OnDrawReleased(Vector2 mouseWorldPosition, DrawSplineController splineController);
}
