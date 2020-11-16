﻿using Assets.AToonWorld.Scripts.PathFinding;
using Assets.AToonWorld.Scripts.PathFinding.Discrete;
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

            var forbiddenSteps = new PathStepsContainer();
            forbiddenSteps.Add(grid[0, 0], grid[1, 1]);
            var minimumPath = grid.FindMinimumPath(start, destination, forbiddenSteps).ToList();
            foreach (var node in minimumPath)
                Debug.Log($"(X,Y) = ({node.X},{node.Y})");            
        }
    }
}
