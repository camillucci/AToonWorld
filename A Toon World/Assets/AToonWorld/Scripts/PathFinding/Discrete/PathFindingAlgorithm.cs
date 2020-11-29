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
			bool NodeCostLessThan(Node a, Node b) => a.FCost <= b.FCost && a.HCost < b.HCost;

			var openNodes = new HashSet<Node> { startNode };
			var closedNodes = new HashSet<Node>();
			
			for (Node curNode = startNode; openNodes.Count > 0 && curNode != destNode; curNode = openNodes.MinimumPoint(NodeCostLessThan))
			{
				foreach (Node neighbor in grid.GetNeighbors(curNode))
					if (neighbor.Walkable && !closedNodes.Contains(neighbor) && !forbiddenSteps.Contains(curNode, neighbor))
					{
						int newCostToNeighbor = curNode.GCost + Distance(curNode, neighbor);
						if (newCostToNeighbor < neighbor.GCost || !openNodes.Contains(neighbor))
						{
							neighbor.GCost = newCostToNeighbor;
							neighbor.HCost = Distance(neighbor, destNode);
							neighbor.Parent = curNode;
							openNodes.Add(neighbor);
						}
					}

				openNodes.Remove(curNode);
				closedNodes.Add(curNode);
			}

			return RetracePath(startNode, destNode);
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
				if (currentNode.Parent != null)
					currentNode = currentNode.Parent;
				else return new List<Node>();
			}
			path.Reverse();

			return path;
		}
	}
}
