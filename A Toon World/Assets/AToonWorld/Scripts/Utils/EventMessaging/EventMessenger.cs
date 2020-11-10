using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace EventMessaging
{
    public class EventMessenger : MonoBehaviour 
    {
        private Dictionary <Events, UnityEvent> _events;
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
            _events = new Dictionary<Events, UnityEvent>();
        }
                
        public void Subscribe(Events event, UnityAction listener)
        {
            if (!eventDictionary.ContainsKey(event))
                events.Add(event, new UnityEvent());
            events[event].AddListener(listener);
        }

        public void UnSubscribe(Events event)
        {
            if (eventDictionary.ContainsKey(event))
                events.Remove(event);
        }

        public void Invoke(Events event, Object... params)
        {
            if (eventDictionary.ContainsKey(event))
                events[event].Invoke(params);
        }
    }
}