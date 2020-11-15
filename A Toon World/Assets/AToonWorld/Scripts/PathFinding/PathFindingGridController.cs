using Assets.AToonWorld.Scripts.Enemies.Breaker;
using Assets.AToonWorld.Scripts.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public class PathFindingGridController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private Vector2 _pathFindingRange;
        [SerializeField] private float _nodeRadius;

        // Private Fields
        private Transform _transform;        
        private bool _awakeCalled;
        private BreakerAreaCollider _breakerAreaCollider;


        // Initialization
        private void Awake()
        {
            _breakerAreaCollider = GetComponent<BreakerAreaCollider>();
            _awakeCalled = true;
            _transform = transform;
            ResetGrid();
        }
       

        private void InitializeColliderSize()
        {
            var width = (NodeToWorldPoint(Grid.TopRight) - NodeToWorldPoint(Grid.TopLeft)).x + 1;
            var height = (NodeToWorldPoint(Grid.TopLeft) - NodeToWorldPoint(Grid.BottomLeft)).y + 1;
            _breakerAreaCollider.SetColliderSize(new Vector2(width, height));
        }



        private void Start()
        {
            InitializeColliderSize();
        }



        // Private Properties
        private Vector2 CurrentPosition => new Vector2(_transform.position.x, transform.position.y);
 

        // Public Properties        
        public Vector2 GridCenter { get; private set; }
        public Vector2 GridOrigin { get; private set; }
        public IGrid Grid { get; private set; }        
        public float NodeRadius => _nodeRadius;


        // Public Methods
        public void ResetGrid()
        {
            var (width, height) = _pathFindingRange / _nodeRadius;            
            Grid = GridFactory.GetNewGrid(Mathf.RoundToInt(width), Mathf.RoundToInt(height));
            GridCenter = CurrentPosition;
            GridOrigin = CurrentPosition - _pathFindingRange / 2;
        }

        public INode WorldPointToNode(Vector2 worldPosition)
        {
            var deltaPos = worldPosition - GridOrigin;
            var (x, y) = deltaPos / _nodeRadius;
            var (intX, intY) = (Mathf.RoundToInt(x), Mathf.RoundToInt(y));
            return Grid[intX, intY];
        }

        public Vector2 NodeToWorldPoint(INode node)
        {
            var deltaPos = _nodeRadius * new Vector2(node.X, node.Y);
            return GridOrigin + deltaPos + new Vector2(_nodeRadius/2, _nodeRadius/2);
        }

        public IEnumerable<Vector3> GetNodeBoundsAndCenter(INode node)
        {
            var nodeCenter =  NodeToWorldPoint(node);
            yield return nodeCenter;
            var deltaPos = new Vector2(_nodeRadius / 2, _nodeRadius / 2);
            yield return deltaPos + nodeCenter;
            yield return nodeCenter - nodeCenter;
            var ortogonalDeltaPos = new Vector2(-deltaPos.y, deltaPos.x);
            yield return nodeCenter + ortogonalDeltaPos;
            yield return nodeCenter - ortogonalDeltaPos;
        }



        // Unity Events

        private void OnDrawGizmos()
        {
            var gridCenter = _awakeCalled ? GridCenter : new Vector2(transform.position.x, transform.position.y);
            Gizmos.DrawWireCube(gridCenter, new Vector3(_pathFindingRange.x, _pathFindingRange.y, 1));
            Gizmos.DrawWireCube(gridCenter, new Vector3(_nodeRadius, _nodeRadius, 1));           
        }
    }
}
