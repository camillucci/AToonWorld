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
        /// <summary>Evento scatenato quando viene selezionato un nuovo inchiostro da periferica</summary>
        public static Event<PlayerInkController.InkType> InkSelected = new Event<PlayerInkController.InkType>();

        /// <summary>Evento scatenato quando cambia la quantità di inchiostro (normalizzato)</summary>
        public static Event<(PlayerInkController.InkType, float)> InkCapacityChanged = new Event<(PlayerInkController.InkType, float)>();

        /// <summary>Evento scatenato quando cambia la quantità di inchiostro (non normalizzato, in quantità assoluta in base all'inchiostro)</summary>
        public static Event<(PlayerInkController.InkType, float)> RawInkCapacityChanged = new Event<(PlayerInkController.InkType, float)>();
        
        /// <summary>Evento scatenato quando viene selezionato un nuovo inchiostro da interfaccia</summary>
        public static Event<PlayerInkController.InkType> InkSelectionRequested = new Event<PlayerInkController.InkType>();
        
        /// <summary>Evento scatenato quando si vuole cambiare il cursore ad esempio entrando in un menu</summary>
        public static Event<CursorController.CursorType> CursorChangeRequest = new Event<CursorController.CursorType>();
    }
}