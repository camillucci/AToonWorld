using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionInkHandler : IInkHandler
{
    private PlayerInkController _playerInkController;

    public ConstructionInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public void OnDrawDown(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        splineController.Clear();
        splineController.AddPoint(mouseWorldPosition);
    }

    public void OnDrawHeld(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        splineController.AddPoint(mouseWorldPosition);
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        splineController.EnableSimulation();
    }
}
