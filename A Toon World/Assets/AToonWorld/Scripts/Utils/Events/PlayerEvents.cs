using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public static class PlayerEvents
    {
        /// <summary>Evento scatenato quando muore il player</summary>
        public static UnityEvent Death = new UnityEvent();

        /// <summary>Evento scatenato quando il player è respawnato</summary>
        public static UnityEvent PlayerRespawned = new UnityEvent();
        
        /// <summary>Evento scatenato quando il player sta respawnando</summary>
        public static UnityEvent PlayerRespawning = new UnityEvent();

        /// <summary>Event notified when a Spline is created in the world</summary>
        public static Event<DrawSplineController> SplineDrawn = new Event<DrawSplineController>();
    }
}