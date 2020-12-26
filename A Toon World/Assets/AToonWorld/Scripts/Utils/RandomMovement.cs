using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Assets.AToonWorld.Scripts.Utils
{
    public class RandomMovement : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] protected float _speed;
        [SerializeField] protected float _movementRadius;


        // Private Fields
        protected Vector2 _start;
        protected Transform _transform;
        protected Vector2 _currentDestination;


        protected virtual void Awake()
        {
            _transform = transform;
            _start = _transform.position;
        }

        protected virtual void Start()
        {
            _currentDestination = _start;
        }




        // UnityEvents
        private void Update()
        {
            if (Vector2.Distance(_currentDestination, _transform.position) < 0.1f)
               _currentDestination = ChangeDestination();

            _transform.position = Vector2.MoveTowards(_transform.position, _currentDestination, _speed * Time.deltaTime);
        }




        // Private Methods
        protected virtual Vector2 ChangeDestination()
        {
            var direction = Random.insideUnitCircle;
            var distance = Random.Range(0, _movementRadius);

            return _start + distance * direction;
        }
    }
}
