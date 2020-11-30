using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts;
using UnityEngine;

[CreateAssetMenu(fileName = "ConstructionInkHandler", menuName = "Inkverse/Inks/Handlers/Construction Ink Handler", order = 3)]
public class ConstructionInkHandler : ScriptableExpendableInkHandler, ISplineInk
{
    private DrawSplineController _boundSplineController;
    private bool _isDrawing = false;

    public DrawSplineController BoundSpline => _boundSplineController;
    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    public override void OnDrawDown(Vector2 mouseWorldPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 0f, LayerMask.GetMask(UnityTag.Player));
        if(!hit)
        {
            _isDrawing = true;
            _boundSplineController.Clear();
            _boundSplineController.AddPoint(mouseWorldPosition);
            _boundSplineController.Color = new Color(this.InkColor.r,
                                                    this.InkColor.g,
                                                    this.InkColor.b,
                                                    0.5f);
        }
    }

    public override bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPosition, Vector2.zero, 0f, LayerMask.GetMask(UnityTag.Player));
        
        //If I can't consume anything, return
        if(_isDrawing && !hit && this.CurrentCapacity > 0)
        {
            Vector2 lastPoint = _boundSplineController.LastPoint;
            Vector2 diff = (mouseWorldPosition - lastPoint);
            float toConsume = diff.magnitude;

            //The added point is a valid point (enough distance from the lastOne)
            if(_boundSplineController.AddPoint(mouseWorldPosition))
            {
                float consumedInk = _expendableResource.Consume(toConsume);

                //If I don't have that much ink, create a smaller segment
                if(consumedInk != toConsume)
                    mouseWorldPosition = lastPoint + diff.normalized * consumedInk;

                _boundSplineController.SetPoint(_boundSplineController.PointCount - 1, mouseWorldPosition);
            }
            return true;
        }
        return false;
    }

    public override void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        if(_boundSplineController.PointCount > 1)
        {
            _boundSplineController.EnableSimulation();
            _boundSplineController.Color = this.InkColor;
        }
        else
            _boundSplineController.gameObject.SetActive(false);
    }
}
