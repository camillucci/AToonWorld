using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Events
{
    public class Event<T> : UnityEvent<T> { }

    public static class InterfaceEvents
    {
        /// <summary>Evento scatenato quando viene selezionato un nuovo inchiostro da periferica</summary>
        public static Event<PlayerInkController.InkType> InkSelected = new Event<PlayerInkController.InkType>();

        /// <summary>Evento scatenato quando cambia la quantità di inchiostro (normalizzato)</summary>
        public static Event<(PlayerInkController.InkType, float)> InkCapacityChanged = new Event<(PlayerInkController.InkType, float)>();
        /// <summary>Evento scatenato quando cambia la quantità di inchiostro (non normalizzato, in quantità assoluta in base all'inchiostro)</summary>
        public static Event<(PlayerInkController.InkType, float)> RawInkCapacityChanged = new Event<(PlayerInkController.InkType, float)>();
        
        /// <summary>Evento scatenato quando viene selezionato un nuovo inchiostro da interfaccia</summary>
        public static Event<PlayerInkController.InkType> InkSelectionRequested = new Event<PlayerInkController.InkType>();
    }
}