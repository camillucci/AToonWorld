using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts;

//TODO: TrailRenderer or Shooting mechanic for this one
public class CancelInkHandler : IInkHandler
{
    private PlayerInkController _playerInkController;
    private List<GameObject> _toDelete;
    private Vector2 _lastPoint;

    public CancelInkHandler(PlayerInkController playerInkController)
    {
        _toDelete = new List<GameObject>();
        _playerInkController = playerInkController;
    }

    public void OnDrawDown(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        _toDelete.Clear();
        splineController.Clear();
        splineController.AddPoint(mouseWorldPosition);
        _lastPoint = mouseWorldPosition;
    }

    public void OnDrawHeld(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        splineController.AddPoint(mouseWorldPosition);
        ProcessToDelete(mouseWorldPosition);
        _lastPoint = mouseWorldPosition;
    }

    public void OnDrawReleased(Vector2 mouseWorldPosition, DrawSplineController splineController)
    {
        ProcessToDelete(mouseWorldPosition);
        
        //TODO: Deletion Animation and Sound?
        _toDelete.ForEach(obj => obj.SetActive(false));
        splineController.gameObject.SetActive(false);
    }

    private void ProcessToDelete(Vector2 currentPoint)
    {
        //TODO: Forse se l'inchiostro è fatto così sarebbe meglio avere un collider invisibile attaccato al mouse invece che fare raycast
        Vector2 diff = _lastPoint - currentPoint;
        RaycastHit2D hit;
        if((hit = Physics2D.Raycast(_lastPoint, diff.normalized, diff.magnitude, LayerMask.GetMask(UnityTag.Drawing))))
            _toDelete.Add(hit.collider.gameObject);
    }
}
