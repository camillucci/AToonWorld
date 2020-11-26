using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public static class AnaliticsEvents
    {
        /// <summary>Evento scatenato quando muore il player</summary>
        public static Event<Vector2> PlayerDeath = new Event<Vector2>();
    }
}