using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public class PathFindingGrid : IGrid
    {
        private readonly PathFinding _pathFinding = new PathFinding();
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

        
        // Indexers
        public INode this[int row, int column] => _nodesMatrix[row, column];



        // Public Methods
        public IEnumerable<Node> GetNeighbours(Node node)
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

        public IEnumerable<INode> GetNeighbours(INode node) => GetNeighbours(node as Node);
        public IEnumerable<INode> FindMinimumPath(INode start, INode destination) => _pathFinding.FindMinimumPath(this, start as Node, destination as Node);
        public int Distance(INode start, INode destination) => _pathFinding.GetDistance(start as Node, destination as Node);
    }
}
