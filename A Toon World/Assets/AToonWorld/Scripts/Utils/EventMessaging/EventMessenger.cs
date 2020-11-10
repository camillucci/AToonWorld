using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Generic;

namespace EventMessaging
{
    public class EventMessenger
    {
        public delegate void Function(params object[] parameters);
        
        private Dictionary <CustomEvents, List<Function>> _events;
        private static EventMessenger _instance;

        public static EventMessenger Instance 
        {
            get
            {
                if (_instance == null)
                    _instance = new EventMessenger();
                return _instance;
            }
        }

        private EventMessenger()
        {
            _events = new Dictionary<CustomEvents, List<Function>>();
        }
                
        public void Subscribe(CustomEvents eventType, Function function)
        {
            if (!_events.ContainsKey(eventType))
                _events.Add(eventType, new List<Function>());
            _events[eventType].Add(function);
        }

        public void UnSubscribe(CustomEvents internalEvent)
        {
            if (_events.ContainsKey(internalEvent))
            {
                _events[internalEvent].Clear();
                _events.Remove(internalEvent);
            }
        }

        public void Invoke(CustomEvents internalEvent)
        {
            if (_events.ContainsKey(internalEvent))
                _events[internalEvent].ForEach(function => function.Invoke());
        }

        public void Invoke(CustomEvents internalEvent, params object[] parameters)
        {
            if (_events.ContainsKey(internalEvent))
                _events[internalEvent].ForEach(function => function.Invoke(parameters));
        }
    }
}