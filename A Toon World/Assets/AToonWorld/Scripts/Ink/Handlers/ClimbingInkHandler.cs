using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "ClimbingInkHandler", menuName = "Inkverse/Inks/Handlers/Climb Ink Handler", order = 2)]
public class ClimbingInkHandler : ScriptableExpendableInkHandler, ISplineInk
{
    private DrawSplineController _boundSplineController;
    private bool _isDrawing = false;
    [SerializeField] private float _sensibility = 0.5f;
    [SerializeField] private float _distanceFromBorder = 0.15f;
    private Vector2 _lastPoint;
    private Bounds _wallBounds;
    private Direction _direction;

    public DrawSplineController BoundSpline => _boundSplineController;
    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    // Returns true if the ink starts drawing, false otherwise
    public override bool OnDrawDown(Vector2 mouseWorldPosition)
    {
        _boundSplineController.Clear(); //Clear even if can't draw
        
        // First point must be on the boarder of a Ground object
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 0f, LayerMask.GetMask(UnityTag.NonWalkable));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(_sensibility, 0), Vector2.zero, 0f, LayerMask.GetMask(UnityTag.NonWalkable));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(-_sensibility, 0), Vector2.zero, 0f, LayerMask.GetMask(UnityTag.NonWalkable));
        if(!hit) hit = Physics2D.Raycast(mouseWorldPosition + new Vector2(0, _sensibility), Vector2.zero, 0f, LayerMask.GetMask(UnityTag.NonWalkable));
        
        // Check if mouse is clicking a Ground object or it is near one, otherwise cancel
        if (hit && hit.collider.gameObject.CompareTag(UnityTag.Ground)) {
            _wallBounds = hit.collider.bounds;
            float leftDistance = Mathf.Abs(_wallBounds.min.x - mouseWorldPosition.x);
            float rightDistance = Mathf.Abs(_wallBounds.max.x - mouseWorldPosition.x);
            float downDistance = Mathf.Abs(_wallBounds.min.y - mouseWorldPosition.y);

            // Check if mouse is near the border of a Ground object, otherwise cancel
            if (leftDistance < rightDistance && leftDistance < _sensibility)
            {
                _lastPoint = new Vector2(_wallBounds.min.x - _distanceFromBorder, mouseWorldPosition.y);
                _direction = Direction.None;
            }
            else if (rightDistance < _sensibility)
            {
                _lastPoint = new Vector2(_wallBounds.max.x + _distanceFromBorder, mouseWorldPosition.y);
                _direction = Direction.None;
            }
            else if (downDistance < _sensibility)
            {
                _lastPoint = new Vector2(mouseWorldPosition.x, _wallBounds.min.y);
                _direction = Direction.Down;
            }
            else
            {
                _boundSplineController.gameObject.SetActive(false);
                return false;
            }
            _isDrawing = true;
            _boundSplineController.AddPoint(_lastPoint);
            _boundSplineController.Color = new Color(this.InkColor.r,
                                                    this.InkColor.g,
                                                    this.InkColor.b,
                                                    0.5f);
            return true;
        }
        _boundSplineController.gameObject.SetActive(false);
        return false;
    }

    public override bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        if (_isDrawing && this.CurrentCapacity > 0) 
        {
            // Check if it is drawing upwards or downwards
            if (_direction == Direction.None && Mathf.Abs(mouseWorldPosition.y - _lastPoint.y) > Mathf.Epsilon)
            {
                _direction = mouseWorldPosition.y < _lastPoint.y ? Direction.Down : Direction.Up;
            }

            // Do not allow to draw from down to up over the nonwalkable object
            if (_direction == Direction.Up && mouseWorldPosition.y > _wallBounds.max.y)
            {
                Vector2 newPoint = new Vector2(_lastPoint.x, _wallBounds.max.y);
                AddPoint(newPoint);
                return false;
            }

            // Add next segment over or under the last point in a straight line
            if (_direction == Direction.Down && mouseWorldPosition.y < _lastPoint.y ||
                _direction == Direction.Up && mouseWorldPosition.y > _lastPoint.y)
            {
                Vector2 newPoint = new Vector2(_lastPoint.x, mouseWorldPosition.y);
                AddPoint(newPoint);
            }
            return true;
        }
        return false;
    }

    // Add point to Spline
    private void AddPoint(Vector2 newPoint)
    {
        float toConsume = Mathf.Abs(_lastPoint.y - newPoint.y);

        if (_boundSplineController.AddPoint(newPoint))
        {
            float consumedInk = _expendableResource.Consume(toConsume);

            // If I don't have that much ink, create a smaller segment
            if(consumedInk != toConsume)
                newPoint = _lastPoint + (_direction == Direction.Down ? Vector2.down : Vector2.up) * consumedInk;

            _boundSplineController.SetPoint(_boundSplineController.PointCount - 1, newPoint);
        }
        _lastPoint = newPoint;
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

    private enum Direction
    {
        None,
        Up,
        Down,
    }
}
