using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts;

//TODO: TrailRenderer or Shooting mechanic for this one
[CreateAssetMenu(fileName = "CancelInkHandler", menuName = "Inkverse/Inks/Handlers/Cancel Ink Handler", order = 4)]
public class CancelInkHandler : ScriptableInkHandler, ISplineInk
{
    private DrawSplineController _boundSplineController;
    private CancelInkObserver _cancelInkObserver;
    private Vector2 _lastPoint;

    public DrawSplineController BoundSpline => _boundSplineController;

    public override bool CanDraw => true;

    public void BindSpline(DrawSplineController splineController)
    {
        _boundSplineController = splineController;
    }

    public override void Init()
    {
        base.Init();
        _cancelInkObserver = FindObjectOfType<CancelInkObserver>();
    }

    public override bool OnDrawDown(Vector2 mouseWorldPosition)
    {
        _boundSplineController.Clear();
        _boundSplineController.AddPoint(mouseWorldPosition);
        _lastPoint = mouseWorldPosition;
        return true;
    }

    public override bool OnDrawHeld(Vector2 mouseWorldPosition)
    {
        _boundSplineController.AddPoint(mouseWorldPosition);
        ProcessToDelete(mouseWorldPosition);
        _lastPoint = mouseWorldPosition;
        return true; //Infinite Ink
    }

    public override void OnDrawReleased(Vector2 mouseWorldPosition)
    {
        ProcessToDelete(mouseWorldPosition);
        _cancelInkObserver.Commit();
        _boundSplineController.gameObject.SetActive(false);
    }

    private void ProcessToDelete(Vector2 currentPoint)
    {
        //TODO: Forse se l'inchiostro è fatto così sarebbe meglio avere un collider invisibile attaccato al mouse invece che fare raycast
        Vector2 diff = _lastPoint - currentPoint;
        Vector2 direction = diff.normalized;
        foreach(RaycastHit2D hit in Physics2D.RaycastAll(_lastPoint, direction, diff.magnitude, LayerMask.GetMask(UnityTag.Drawing)))
            _cancelInkObserver.NotifyDelete(hit.collider.gameObject, hit.point, direction);
    }
}
