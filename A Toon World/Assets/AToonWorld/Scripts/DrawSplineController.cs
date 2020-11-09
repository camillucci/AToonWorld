using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
[RequireComponent(typeof(EdgeCollider2D))]
//[RequireComponent(typeof(Rigidbody2D))]
public class DrawSplineController : MonoBehaviour
{
    //FIXME: La generazione della spline è lentissima e sembra avere problemi di performance
    //TODO: Generalizzare la cosa per decidere in futuro se usare SpriteShape o LineRenderer
    //FIXME: La fisica con Rigidbody2D attualmente è rotta sulle linee (probabilmente perchè assume che l'origine dell'oggetto sia il centro di massa?)
    private SpriteShapeController _inkSpriteShape;
    //private Rigidbody2D _splineRigidBody;
    private Collider2D _splineCollider;
    private Transform _splineTransform;
    private int _currentSplineIndex;
    //[SerializeField] private float _stiffness = 1.0f;
    [SerializeField] private float _minDistanceBetweenPoints = float.Epsilon;

    void Awake()
    {
        _inkSpriteShape = GetComponent<SpriteShapeController>();
        //_splineRigidBody = GetComponent<Rigidbody2D>();
        _splineTransform = GetComponent<Transform>();
        _splineCollider = GetComponent<EdgeCollider2D>();
        this.Clear();
    }

    public void Clear()
    {
        //_splineRigidBody.simulated = false;
        _inkSpriteShape.spline.Clear();
        _splineCollider.enabled = false;
        _splineTransform.position = Vector3.zero;
        _splineTransform.rotation = Quaternion.identity;
        _currentSplineIndex = 0;
    }

    public void AddPoint(Vector2 newPoint)
    {
        //First point
        if(_currentSplineIndex == 0)
        {
            _inkSpriteShape.spline.InsertPointAt(_currentSplineIndex++, newPoint);
            return;
        }

        //Spline points
        Vector2 lastPoint = _inkSpriteShape.spline.GetPosition(_currentSplineIndex - 1);
        
        if(IsValidPoint(newPoint, lastPoint))
        {
            //La parte commentata gestirebbe le tangenti tra segmenti diversi, per ora essendo già lenta la generazione così eviterei
            //Vector2 danglePos = (lastPoint + newPoint) / 2;
            //_inkSpriteShape.spline.InsertPointAt(_currentSplineIndex, danglePos); //dangle point
            _inkSpriteShape.spline.InsertPointAt(_currentSplineIndex++, newPoint); //end point

            //Vector2 diff = newPoint - lastPoint;

            //float dangle = diff.magnitude / _stiffness;

            //
            //danglePos.y -= dangle / 2;

            //Vector2 averageDir = ((lastPoint - danglePos) - (newPoint - danglePos)) / 2;
            //_inkSpriteShape.spline.SetLeftTangent(_currentSplineIndex, averageDir / 2);
            //_inkSpriteShape.spline.SetRightTangent(_currentSplineIndex, -averageDir / 2);
            //_inkSpriteShape.spline.SetTangentMode(_currentSplineIndex, ShapeTangentMode.Continuous);

            //_currentSplineIndex++;
        }
    }

    public void EnableSimulation()
    {
        _splineCollider.enabled = true;
        //TODO: Custom Physics?
        //_splineRigidBody.simulated = true;
    }

    bool IsValidPoint(Vector2 point, Vector2 lastPoint)
    {
        float distance = (point - lastPoint).magnitude;
        return distance > _minDistanceBetweenPoints;
    }
}
