using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.AToonWorld.Scripts.Level
{    
    public class BubbleCollectible : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed;
        [SerializeField] private float _movementRadius;


        // Private Fields
        private Vector2 _start;
        private Transform _transform;
        private Vector2 _currentDestination;
        private Collectible _collectible;


        private void Awake()
        {
            _transform = transform;
            _start = _transform.position;
        }

        private void Start()
        {
            _currentDestination = _start;
            _collectible = GetComponentInChildren<Collectible>();
            _collectible.PlayerHit += _ => gameObject.SetActive(false);
        }
        



        // UnityEvents
        private void Update()
        {
            if (Vector2.Distance(_currentDestination, _transform.position) < float.Epsilon)
                ChangeDestination();

            _transform.position = Vector2.MoveTowards(_transform.position, _currentDestination, _speed * Time.deltaTime);
        }




        // Private Methods
        private void ChangeDestination()
        {
            var direction = Random.insideUnitCircle;
            var distance = Random.Range(0, _movementRadius);

            _currentDestination = _start +  distance * direction;
        }


    }
}
