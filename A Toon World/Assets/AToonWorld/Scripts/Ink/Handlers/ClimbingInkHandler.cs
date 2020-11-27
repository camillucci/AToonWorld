using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "ClimbingInkHandler", menuName = "Inkverse/Inks/Handlers/Climb Ink Handler", order = 2)]
public class ClimbingInkHandler : ScriptableExpendableInkHandler, ISplineInk
{
    private DrawSplineController _boundSplineController;
    private bool _isDrawing = false;
    [SerializeField] private float _sensibilty = 0.5f;
    [SerializeField] private float _distanceFromBorder = 0.05f;
    private Vector2 _lastPoint;

    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    public override void OnDrawDown(Vector2 mouseWorldPosition)
    {
        // First point must be on the boarder of a Ground object
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, LayerMask.GetMask(UnityTag.Default));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(_sensibility, 0), Vector2.zero, LayerMask.GetMask(UnityTag.Default));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(-_sensibility, 0), Vector2.zero, LayerMask.GetMask(UnityTag.Default));
        
        // Check if mouse is clicking a Ground object or it is near one, otherwise cancel
        if (hit && hit.collider.gameObject.CompareTag(UnityTag.Ground)) {
            Bounds wallBounds = hit.collider.bounds;
            float leftDistance = Mathf.Abs(wallBounds.min.x - mouseWorldPosition.x);
            float rightDistance = Mathf.Abs(wallBounds.max.x - mouseWorldPosition.x);
            float downDistance = Mathf.Abs(wallBounds.min.y - mouseWorldPosition.y);

            // Check if mouse is near the border of a Ground object, otherwise cancel
            if (leftDistance < rightDistance && leftDistance < _sensibility)
                _lastPoint = new Vector2(wallBounds.min.x - _distanceFromBorder, mouseWorldPosition.y);
            else if (rightDistance < _sensibility)
                _lastPoint = new Vector2(wallBounds.max.x + _distanceFromBorder, mouseWorldPosition.y);
            else if (downDistance < _sensibility)
                _lastPoint = mouseWorldPosition;
            else
            {
                _boundSplineController.gameObject.SetActive(false);
                return;
            }
            _isDrawing = true;
            _boundSplineController.Clear();
            _boundSplineController.AddPoint(_lastPoint);
            _boundSplineController.Color = new Color(this.InkColor.r,
                                                    this.InkColor.g,
                                                    this.InkColor.b,
                                                    0.5f);
        }
        else
        {
            _boundSplineController.gameObject.SetActive(false);
        }
    }

    public override bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        if (_isDrawing && this.CurrentCapacity > 0) 
        {
            // Only add segment if the next point is under the last point
            if(mouseWorldPosition.y < _lastPoint.y) 
            {
                Vector2 newPoint = new Vector2(_lastPoint.x, mouseWorldPosition.y);
                float toConsume = _lastPoint.y - newPoint.y;

                if(_boundSplineController.AddPoint(newPoint))
                {
                    float consumedInk = _expendableResource.Consume(toConsume);

                    //If I don't have that much ink, create a smaller segment
                    if(consumedInk != toConsume)
                        newPoint = _lastPoint + Vector2.down * consumedInk;

                    _boundSplineController.SetPoint(_boundSplineController.PointCount - 1, newPoint);
                }
                _lastPoint = newPoint;
            }
            return true;
        }
        return false;
    }

    public override void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        if (_isDrawing)
        {
            _isDrawing = false;
            if(_boundSplineController.PointCount > 1)
            {
                _boundSplineController.EnableSimulation();
                _boundSplineController.Color = this.InkColor;
            }
            else
                _boundSplineController.gameObject.SetActive(false);
        }
    }
}
