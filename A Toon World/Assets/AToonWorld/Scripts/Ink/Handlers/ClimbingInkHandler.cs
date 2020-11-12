using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

public class ClimbingInkHandler : IInkHandler, ISplineInk
{
    private PlayerInkController _playerInkController;
    private DrawSplineController _boundSplineController;
    private bool _isDrawing = false;
    private const float _sensibilty = 0.5f;
    private const float _distanceFromBorder = 0.05f;
    private Vector2 _lastPoint;

    public ClimbingInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    public void OnDrawDown(Vector2 mouseWorldPosition)
    {
        // First point must be on the boarder of a Ground object
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, LayerMask.GetMask(UnityTag.Default));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(_sensibilty, 0), Vector2.zero, LayerMask.GetMask(UnityTag.Default));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(-_sensibilty, 0), Vector2.zero, LayerMask.GetMask(UnityTag.Default));
        
        if (hit && hit.collider.gameObject.CompareTag(UnityTag.Ground)) {
            _isDrawing = true;
            Bounds wallBounds = hit.collider.bounds;
            if (Mathf.Abs(wallBounds.min.x - mouseWorldPosition.x) < _sensibilty)
                _lastPoint = new Vector2(wallBounds.min.x - _distanceFromBorder, mouseWorldPosition.y);
            else if (Mathf.Abs(wallBounds.max.x - mouseWorldPosition.x) < _sensibilty)
                _lastPoint = new Vector2(wallBounds.max.x + _distanceFromBorder, mouseWorldPosition.y);
            else
                _lastPoint = mouseWorldPosition;
            _boundSplineController.Clear();
            _boundSplineController.AddPoint(_lastPoint);
        }
        else
        {
            _boundSplineController.gameObject.SetActive(false);
        }
    }

    public bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        if (_isDrawing && mouseWorldPosition.y < _lastPoint.y) {
            Vector2 newPoint = new Vector2(_lastPoint.x, mouseWorldPosition.y);
            _boundSplineController.AddPoint(newPoint);
            _lastPoint = newPoint;
        }
        return true;
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        if (_isDrawing)
        {
            _isDrawing = false;
            if(_boundSplineController.PointCount > 1)
                _boundSplineController.EnableSimulation();
            else
                _boundSplineController.gameObject.SetActive(false);
        }
    }
}
