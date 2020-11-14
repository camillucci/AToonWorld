using Assets.AToonWorld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public class BreakerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private Vector2 _pathFindingRange;
        [SerializeField] private float _sensibilityRadius;


        // Private Fields
        private Transform _transform;
        private IGrid _grid;
        private bool _awakeCalled;
        


        // Initialization
        private void Awake()
        {
            _awakeCalled = true;
            _transform = transform;
            ResetGrid();
        }

        private void Start()
        {
            
        }

 

        // Public Properties
        public Vector2 CurrentPosition => new Vector2(_transform.position.x, transform.position.y);
        public Vector2 GridCenter { get; private set; }
        public Vector2 GridOrigin { get; private set; }


        // Public Methods        




        // Unity Events

        private void Update()
        {
            _transform.position = NodeToWorldPoint(WorldPointToNode(GridOrigin));
        }

        private void OnDrawGizmos()
        {
            var gridCenter = _awakeCalled ? GridCenter : new Vector2(transform.position.x, transform.position.y);
            Gizmos.DrawWireCube(gridCenter, new Vector3(_pathFindingRange.x, _pathFindingRange.y, 1));
            Gizmos.DrawWireCube(gridCenter, new Vector3(_sensibilityRadius, _sensibilityRadius, 1));           
        }



        // Private Methods
        private void ResetGrid()
        {
            var (width, height) = _pathFindingRange / _sensibilityRadius;
            _grid = GridFactory.GetNewGrid((int)width, (int)height);
            GridCenter = CurrentPosition;
            GridOrigin = CurrentPosition - _pathFindingRange / 2;
        }

        private INode WorldPointToNode(Vector2 worldPosition)
        {
            var deltaPos = worldPosition - GridOrigin;
            var (x, y) = deltaPos / _sensibilityRadius;
            return _grid[(int)x, (int)y];
        }

        private Vector3 NodeToWorldPoint(INode node)
        {
            var deltaPos = _sensibilityRadius * new Vector2(node.X, node.Y);
            return GridOrigin + deltaPos;
        }
    }
}
