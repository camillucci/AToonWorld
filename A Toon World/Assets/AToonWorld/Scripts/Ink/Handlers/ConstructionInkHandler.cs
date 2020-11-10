using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionInkHandler : IInkHandler, ISplineInk
{
    private PlayerInkController _playerInkController;
    private DrawSplineController _boundSplineController;

    public ConstructionInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    public void OnDrawDown(Vector2 mouseWorldPosition)
    {
        _boundSplineController.Clear();
        _boundSplineController.AddPoint(mouseWorldPosition);
    }

    public void OnDrawHeld(Vector2 mouseWorldPosition)
    {
        _boundSplineController.AddPoint(mouseWorldPosition);
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        _boundSplineController.EnableSimulation();
    }
}
