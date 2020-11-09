using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.AToonWorld.Scripts.Utils;

public class BuildLineController : MonoBehaviour
{
    private bool clicked; 
    private List<Vector2> points;
    private float minDistanceFromTwoFollowingPoints = 0;

    void Awake() 
    {
        this.points = new List<Vector2>();
    }

    void Start()
    {
        clicked = false;
    }

    void Update()
    {
        if (InputUtils.DrawDown)
        {
            //When mouse click down
            clicked = true;
            points.Clear();
            points.Add(Input.mousePosition);
        }

        if (InputUtils.DrawHeld)
        {
            //During mouse button is clicked;
            clicked = true;
            if (IsValidPoint(Input.mousePosition))
                points.Add(Input.mousePosition);
        }

        if (!InputUtils.DrawHeld && clicked)
        {
            //When mouse button is released
            clicked = false;
            // ! creare l'oggetto partendo dalla lista di punti
            Debug.Log($"punti catturati: {points.Count}");
        }
    }

    bool IsValidPoint(Vector2 point)
    {
        float distance = (point - points[points.Count - 1]).magnitude;
        return distance > minDistanceFromTwoFollowingPoints;
    }
}
