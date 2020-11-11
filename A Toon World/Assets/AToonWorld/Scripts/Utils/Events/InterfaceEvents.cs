using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace Events
{
    public class Event<T> : UnityEvent<T> { }

    public static class InterfaceEvents
    {
        /// <summary>Evento scatenato quando viene selezionato un nuovo inchiostro</summary>
        public static Event<PlayerInkController.InkType> InkSelected = new Event<PlayerInkController.InkType>();
    }
}