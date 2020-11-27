using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public class PathFindingGrid : IGrid
    {
        private readonly PathFindingAlgorithm _pathFinding = new PathFindingAlgorithm();
        private Node[,] _nodesMatrix;



        // Initialization
        public PathFindingGrid(int width, int height)
        {
            _nodesMatrix = new Node[width, height];
            (Width, Height) = (width, height);
            BuildNodes();
        }

        private void BuildNodes()
        {
            for (var i = 0; i < _nodesMatrix.GetLength(0); i++)
                for (var j = 0; j < _nodesMatrix.GetLength(1); j++)
                   _nodesMatrix[i,j] = BuildNode(i, j);
        }

        private Node BuildNode(int x, int y)
        {
            return new Node(x, y);
        }



        // Public Properties
        public int Width { get; private set; }
        public int Height { get; private set; }
        public INode BottomLeft => _nodesMatrix[0, 0];
        public INode BottomRight => _nodesMatrix[Width - 1, 0];
        public INode TopLeft => _nodesMatrix[0, Height - 1];
        public INode TopRight => _nodesMatrix[Width - 1, Height - 1];



        // Indexers
        public INode this[int row, int column] => _nodesMatrix[row, column];
        public INode this[(int row, int column) coordinates] => this[coordinates.row, coordinates.column];



        // Public Methods
        public IEnumerable<Node> GetNeighbors(Node node)
        {
            bool IsBetween(int from, int length, int val) => from <= val && val < from + length;

            for (int deltaX = -1; deltaX <= 1; deltaX++)
                for (int deltaY = -1; deltaY <= 1; deltaY++)
                    if ((deltaX, deltaY) != (0, 0))
                    {
                        var (x, y) = (node.X + deltaX, node.Y + deltaY);                        
                        if (IsBetween(0, Width, x) && IsBetween(0, Height, y))
                            yield return _nodesMatrix[x,y];
                    }
        }

        public IEnumerable<INode> GetNeighbors(INode node) 
            => GetNeighbors(node as Node);
        
        public IEnumerable<INode> FindMinimumPath(INode start, INode destination)
            => _pathFinding.FindMinimumPath(this, start as Node, destination as Node);
        
        public int Distance(INode start, INode destination) 
            => _pathFinding.Distance(start as Node, destination as Node);
        
        public IEnumerable<INode> FindMinimumPath(INode start, INode destination, PathStepsContainer forbiddenSteps)
           => _pathFinding.FindMinimumPath(this, start as Node, destination as Node, forbiddenSteps);

        public bool TryGetValue(int x, int y, out INode node)
        {
            bool isInside = (x >= 0 && x < Width) && (y >= 0 && y < Height);
            if (isInside)
            {
                node = this[x, y];
                return true;
            }
            else
            {
                node = null;
                return false;
            }
        }



        // IEnumerable implementation
        public IEnumerator<INode> GetEnumerator()
        {
            foreach (var node in _nodesMatrix)
                yield return node;
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();       
    }
}
