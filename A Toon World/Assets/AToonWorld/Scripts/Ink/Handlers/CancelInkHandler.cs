using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts;

//TODO: TrailRenderer or Shooting mechanic for this one
public class CancelInkHandler : IInkHandler, ISplineInk
{
    private PlayerInkController _playerInkController;
    private DrawSplineController _boundSplineController;
    private Vector2 _lastPoint;

    public CancelInkHandler(PlayerInkController playerInkController)
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
        _lastPoint = mouseWorldPosition;
    }

    public bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        _boundSplineController.AddPoint(mouseWorldPosition);
        ProcessToDelete(mouseWorldPosition);
        _lastPoint = mouseWorldPosition;
        return true; //Infinite Ink
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        ProcessToDelete(mouseWorldPosition);
        CancelInkObserver.Instance.Commit();
        _boundSplineController.gameObject.SetActive(false);
    }

    private void ProcessToDelete(Vector2 currentPoint)
    {
        //TODO: Forse se l'inchiostro è fatto così sarebbe meglio avere un collider invisibile attaccato al mouse invece che fare raycast
        Vector2 diff = _lastPoint - currentPoint;
        Vector2 direction = diff.normalized;
        RaycastHit2D hit;
        if((hit = Physics2D.Raycast(_lastPoint, direction, diff.magnitude, LayerMask.GetMask(UnityTag.Drawing))))
            CancelInkObserver.Instance.NotifyDelete(hit.collider.gameObject, hit.point, direction);
    }
}
