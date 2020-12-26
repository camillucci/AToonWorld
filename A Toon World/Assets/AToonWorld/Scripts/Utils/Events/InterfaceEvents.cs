using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;
using Assets.AToonWorld.Scripts.UI;

namespace Events
{
    public static class InterfaceEvents
    {
        /// <summary>Event raised when a new ink is selected from a peripheral device (1 2 3 4 or middle mouse button)</summary>
        public static Event<PlayerInkController.InkType> InkSelected = new Event<PlayerInkController.InkType>();

        /// <summary>Event raised when the quantity of ink changes (normalized)</summary>
        public static Event<(PlayerInkController.InkType, float)> InkCapacityChanged = new Event<(PlayerInkController.InkType, float)>();

        /// <summary>Event raised when the quantity of ink changes (not normalized, absolute quantity depending on the InkType)</summary>
        public static Event<(PlayerInkController.InkType, float)> RawInkCapacityChanged = new Event<(PlayerInkController.InkType, float)>();
        
        /// <summary>Event raised when a new ink is selected from the interaface (InkWheel with right mouse button)</summary>
        public static Event<PlayerInkController.InkType> InkSelectionRequested = new Event<PlayerInkController.InkType>();
        
        /// <summary>Event raised when we want to change the cursor sprite, when we enter a level or a menu</summary>
        public static Event<CursorController.CursorType> CursorChangeRequest = new Event<CursorController.CursorType>();
    }
}