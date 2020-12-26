using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public static class PlayerEvents
    {
        /// <summary>Event raised when the player dies</summary>
        public static UnityEvent Death = new UnityEvent();

        /// <summary>Event raised when the player has finished respawning</summary>
        public static UnityEvent PlayerRespawned = new UnityEvent();
        
        /// <summary>Event raised when the player starts respawning</summary>
        public static UnityEvent PlayerRespawning = new UnityEvent();

        /// <summary>Event raised when a Spline is created in the world</summary>
        public static Event<DrawSplineController> SplineDrawn = new Event<DrawSplineController>();
    }
}