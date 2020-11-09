using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils;

public class BuildLineController : MonoBehaviour
{
    //TODO: Ink Pooling (probabilmente questa classe sparirà)
    [SerializeField] private DrawSplineController _drawSplineController;

    private Vector2 _mouseWorldPosition;
    void Update()
    {
        if (InputUtils.DrawDown)
        {
            _drawSplineController.Clear();
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _drawSplineController.AddPoint(_mouseWorldPosition);
        }

        if (InputUtils.DrawHeld)
        {
            _mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            _drawSplineController.AddPoint(_mouseWorldPosition);
        }

        if (InputUtils.DrawUp)
        {
            _drawSplineController.EnableSimulation();
        }

        //if (!InputUtils.DrawHeld && clicked)
        //{
        //    //When mouse button is released
        //    clicked = false;
        //    // ! creare l'oggetto partendo dalla lista di punti
        //    Debug.Log($"punti catturati: {points.Count}");
        //}
    }
}
