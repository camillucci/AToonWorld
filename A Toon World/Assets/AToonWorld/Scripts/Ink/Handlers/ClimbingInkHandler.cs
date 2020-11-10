using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class ClimbingInkHandler : IInkHandler
{
    private PlayerInkController _playerInkController;
    private bool _isDrawing = false;
    private Vector2 _lastPoint;

    public ClimbingInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public void OnDrawDown(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        // First point must be on the boarder of a Ground object
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 500, LayerMask.GetMask(UnityTag.Default));
        if (hit && hit.collider.gameObject.CompareTag(UnityTag.Ground)) {
            _isDrawing = true;
            Bounds wallBounds = hit.collider.bounds;
            if (Mathf.Abs(wallBounds.min.x - mouseWorldPosition.x) < 0.5f)
                _lastPoint = new Vector2(wallBounds.min.x - 0.1f, mouseWorldPosition.y);
            else if (Mathf.Abs(wallBounds.max.x - mouseWorldPosition.x) < 0.5f)
                _lastPoint = new Vector2(wallBounds.max.x + 0.1f, mouseWorldPosition.y);
            else
                _lastPoint = mouseWorldPosition;
            splineController.Clear();
            splineController.AddPoint(_lastPoint);
        }
        else
        {
            splineController.gameObject.SetActive(false);
        }
    }

    public void OnDrawHeld(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        if (_isDrawing && mouseWorldPosition.y < _lastPoint.y) {
            Vector2 newPoint = new Vector2(_lastPoint.x, mouseWorldPosition.y);
            splineController.AddPoint(newPoint);
            _lastPoint = newPoint;
        }
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        if (_isDrawing)
        {
            splineController.EnableSimulation();
            _isDrawing = false;
        }
    }
}
