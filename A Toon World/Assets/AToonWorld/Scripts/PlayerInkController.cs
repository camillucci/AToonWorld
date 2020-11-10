using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils;

public class PlayerInkController : MonoBehaviour
{
    //TODO: Ink Pooling (probabilmente questa classe sparirà)
    //TODO: Logica selezione ink
    //TODO: Interfaccia e gestione ink
    [SerializeField] private DrawSplineController _drawSplineController;

    private Vector2 _mouseWorldPosition;

    public void OnDrawDown()
    {
        _drawSplineController.Clear();
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _drawSplineController.AddPoint(_mouseWorldPosition);
    }

    public void WhileDrawHeld()
    {
        _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _drawSplineController.AddPoint(_mouseWorldPosition);
    }

    public void OnDrawReleased()
    {
        _drawSplineController.EnableSimulation();
    }
}
