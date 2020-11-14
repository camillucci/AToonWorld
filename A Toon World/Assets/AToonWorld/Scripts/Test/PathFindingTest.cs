using Assets.AToonWorld.Scripts.PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Test
{
    public class PathFindingTest : MonoBehaviour
    {
        private void Awake()
        {
            Test();
        }


        public void Test()
        {
            IGrid grid = new PathFindingGrid(4,5);
            var start = grid[0, 0];
            var destination = grid[2, 3];



            var notWalkable = new INode[] 
            {
                grid[0, 1], grid[1, 2], grid[2,2], grid[3,2]
            };
            foreach (var node in notWalkable)
                node.Walkable = false;

            var minimumPath = grid.FindMinimumPath(start, destination).ToList();
            foreach (var node in minimumPath)
                Debug.Log($"(X,Y) = ({node.X},{node.Y})");            
        }
    }
}
