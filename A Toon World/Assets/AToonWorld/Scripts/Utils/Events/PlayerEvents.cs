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

        /// <summary>Event notified when a Spline is created in the world</summary>
        public static Event<DrawSplineController> SplineDrawn = new Event<DrawSplineController>();
    }
}