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
			bool NodeCostLessThan(Node a, Node b) => a.FCost < b.FCost && a.HCost < b.HCost;			
			HashSet<Node> openNodes = new HashSet<Node> { startNode };
			HashSet<Node> closedNodes = new HashSet<Node>();

			for(Node currentNode = startNode; openNodes.Any() && currentNode != destNode; currentNode = openNodes.MinimumPointOrDefault(NodeCostLessThan))
			{
				foreach (Node neighbour in grid.GetNeighbours(currentNode))
					if (neighbour.Walkable && !forbiddenSteps.Contains(currentNode, neighbour) && !closedNodes.Contains(neighbour) )
					{
						int newCostToNeighbour = currentNode.GCost + GetDistance(currentNode, neighbour);
						if (newCostToNeighbour < neighbour.GCost || !openNodes.Contains(neighbour))
						{
							neighbour.GCost = newCostToNeighbour;
							neighbour.HCost = GetDistance(neighbour, destNode);
							neighbour.Parent = currentNode;
							openNodes.Add(neighbour);
						}
					}
				openNodes.Remove(currentNode);
				closedNodes.Add(currentNode);
			}

			return RetracePath(startNode, destNode);
		}

		
		// Public Methods
		public int GetDistance(Node nodeA, Node nodeB)
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

			for (Node currentNode = endNode; currentNode != startNode; currentNode = currentNode.Parent)
				path.Add(currentNode);				

			return path.ReverseAsEnumerable();
		}
	}
}
