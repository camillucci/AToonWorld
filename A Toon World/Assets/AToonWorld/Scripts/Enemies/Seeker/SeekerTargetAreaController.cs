using Assets.AToonWorld.Scripts.PathFinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Seeker
{
    public class SeekerTargetAreaController : TargetAreaController
    {

        // Initialization
        protected override void Start()
        {
            base.Start();
            InitializeObstacles();
        }

        private void InitializeObstacles()
        {
            var obstaclesTags = new string[] { UnityTag.Ground };
            foreach (var tag in obstaclesTags)
                AddStaticObstacle(tag);
        }
    }
}
