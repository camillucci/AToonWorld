using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding.Discrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.PathFinding
{
    public class PathFindingAlgorithm
    {
		// Public Methods

		public IEnumerable<Node> FindMinimumPath(PathFindingGrid grid, Node startNode, Node destNode)
			=> FindMinimumPath(grid, startNode, destNode, new PathStepsContainer());

		public IEnumerable<Node> FindMinimumPath(PathFindingGrid grid, Node startNode, Node destNode, PathStepsContainer forbiddenSteps)
		{
			bool NodeCostLessThan(Node a, Node b) => a.GCost <= b.GCost && a.HCost < b.HCost;

			var openNodes = new HashSet<Node> { startNode };
			var closedNodes = new HashSet<Node>();

			Node curNode;
			for (curNode = startNode; openNodes.Count > 0 && curNode != destNode; curNode = openNodes.MinimumPoint(NodeCostLessThan))
			{
				foreach (Node neighbour in grid.GetNeighbors(curNode))
					if (neighbour.Walkable && !closedNodes.Contains(neighbour) && !forbiddenSteps.Contains(curNode, neighbour))
					{
						int newCostToNeighbor = curNode.GCost + Distance(curNode, neighbour);
						if (newCostToNeighbor < neighbour.GCost || !openNodes.Contains(neighbour))
						{
							neighbour.GCost = newCostToNeighbor;
							neighbour.HCost = Distance(neighbour, destNode);
							neighbour.Parent = curNode;
							openNodes.Add(neighbour);
						}
					}

				openNodes.Remove(curNode);
				closedNodes.Add(curNode);
			}

			return curNode == destNode ? RetracePath(startNode, destNode) : new List<Node>();
		}

		
		// Public Methods
		public int Distance(Node nodeA, Node nodeB)
		{
			int distanceX = Mathf.Abs(nodeA.X - nodeB.X);
			int distanceY = Mathf.Abs(nodeA.Y - nodeB.Y);

            int DistanceOf(int minDistance, int maxDistance)
				=> 14 * minDistance + 10 * (maxDistance - minDistance);

			return distanceX > distanceY ? DistanceOf(distanceY, distanceX) : DistanceOf(distanceX, distanceY);
		}

		
		// Private Methods
		private IEnumerable<Node> RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();

			Node currentNode = endNode;
			while(currentNode != startNode)
            {
				path.Add(currentNode);				
				currentNode = currentNode.Parent;
			}
			path.Reverse();

			return path;
		}
	}
}
