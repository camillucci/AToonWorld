using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public static class LevelEvents
    {
        /// <summary>Event notified when a Spline is created in the world</summary>
        public static Event<DrawSplineController> SplineDrawn = new Event<DrawSplineController>();

        /// <summary>Event notified when a checkpoint is reached</summary>
        public static Event<int> CheckpointReached = new Event<int>();

        /// <summary>Event notified when an enemy is killed</summary>
        public static Event<GameObject> EnemyKilled = new Event<GameObject>();
    }
}