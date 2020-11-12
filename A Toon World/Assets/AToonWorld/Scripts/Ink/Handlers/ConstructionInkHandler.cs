using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionInkHandler : ExpendableResource, IInkHandler, ISplineInk
{
    private PlayerInkController _playerInkController;
    private DrawSplineController _boundSplineController;

    public ConstructionInkHandler(PlayerInkController playerInkController)
    {
        _playerInkController = playerInkController;
    }

    public override float MaxCapacity => 100.0f;

    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    public void OnDrawDown(Vector2 mouseWorldPosition)
    {
        _boundSplineController.Clear();
        _boundSplineController.AddPoint(mouseWorldPosition);
    }

    public override void SetCapacity(float newCapacity)
    {
        base.SetCapacity(newCapacity);
        Events.InterfaceEvents.InkCapacityChanged.Invoke((PlayerInkController.InkType.Construction, newCapacity/MaxCapacity));
    }

    
    public bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        Vector2 lastPoint = _boundSplineController.LastPoint;
        Vector2 diff = (mouseWorldPosition - lastPoint);
        float toConsume = diff.magnitude;
        
        //If I can't consume anything, return
        if(this._capacity > 0.0f)
        {
            //The added point is a valid point (enough distance from the lastOne)
            if(_boundSplineController.AddPoint(mouseWorldPosition))
            {
                float consumedInk = this.Consume(toConsume);

                //If I don't have that much ink, create a smaller segment
                if(consumedInk != toConsume)
                    mouseWorldPosition = lastPoint + diff.normalized * consumedInk;

                _boundSplineController.SetPoint(_boundSplineController.PointCount - 1, mouseWorldPosition);
            }
            return true;
        }
        return false;
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        if(_boundSplineController.PointCount > 1)
            _boundSplineController.EnableSimulation();
        else
            _boundSplineController.gameObject.SetActive(false);
    }
}
