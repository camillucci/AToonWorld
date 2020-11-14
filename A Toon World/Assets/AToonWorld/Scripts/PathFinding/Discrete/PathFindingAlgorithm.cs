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

		public IList<Node> FindMinimumPath(PathFindingGrid grid, Node startNode, Node destNode)
		{
			IList<Node> path = new List<Node>();
			List<Node> openSet = new List<Node>();
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);			
			while (openSet.Count > 0 && !path.Any())
			{
				Node toCloseNode = openSet[0];
				for (int i = 1; i < openSet.Count; i++)
					if (openSet[i].FCost < toCloseNode.FCost || openSet[i].FCost == toCloseNode.FCost)
						if (openSet[i].HCost < toCloseNode.HCost)
							toCloseNode = openSet[i];

				openSet.Remove(toCloseNode);
				closedSet.Add(toCloseNode);

				if (toCloseNode == destNode)
					path = RetracePath(startNode, destNode);
				else
					foreach (Node neighbour in grid.GetNeighbours(toCloseNode))
						if (neighbour.Walkable && !closedSet.Contains(neighbour))
						{
							int newCostToNeighbour = toCloseNode.GCost + GetDistance(toCloseNode, neighbour);
							if (newCostToNeighbour < neighbour.GCost || !openSet.Contains(neighbour))
							{
								neighbour.GCost = newCostToNeighbour;
								neighbour.HCost = GetDistance(neighbour, destNode);
								neighbour.Parent = toCloseNode;

								if (!openSet.Contains(neighbour))
									openSet.Add(neighbour);
							}
						}
			}

			return path;
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
		private IList<Node> RetracePath(Node startNode, Node endNode)
		{
			List<Node> path = new List<Node>();

			for (Node currentNode = endNode; currentNode != startNode; currentNode = currentNode.Parent)
				path.Add(currentNode);				
			path.Reverse();

			return path;
		}
	}
}
