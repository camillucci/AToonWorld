using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DrawSplineController))]
public class SplineGenerator : MonoBehaviour
{
    private DrawSplineController _drawSplineController;
    private Vector3 _startingPosition;
    private float _timePassed;
    [SerializeField] private float _speed = 1.0f;
    void Awake()
    {
        _startingPosition = transform.position;
        _timePassed = 0.0f;
        _drawSplineController = GetComponent<DrawSplineController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _drawSplineController.AddPoint(transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        _timePassed += Time.deltaTime;
        _drawSplineController.AddPoint(_startingPosition + 
                                        (transform.up * Mathf.Sin(_timePassed * _speed)) +
                                        (transform.right * _timePassed * _speed));
    }
}
