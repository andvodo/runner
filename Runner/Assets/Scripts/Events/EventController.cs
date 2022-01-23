using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Events
{
    public static class EventController
    {

        private static List<EventSubscription> _eventSubscriptions = new List<EventSubscription>();
        private static readonly object _lockObject = new object();
        
        public static void CallEvent(Event newEvent)
        {
            List<Action<Event>> callbacks = new List<Action<Event>>();
            
            lock (_lockObject)
            {
                string eventType = newEvent.GetType().ToString();
                foreach (var subscription in _eventSubscriptions.Where(subscription => subscription.EventType == eventType))
                {
                    callbacks.Add(subscription.Callback);
                }
            }
            
            foreach (Action<Event> callback in callbacks)
            {
                callback?.Invoke(newEvent);
            }
        }

        public static EventSubscription Subscribe(EventSubscription subscription)
        {
            lock (_lockObject)
            {
                if (!_eventSubscriptions.Contains(subscription)) 
                    _eventSubscriptions.Add(subscription);
            }
            return subscription;
        }
    
        public static void Unsubscribe(EventSubscription subscription)
        {
            lock (_lockObject)
            {
                _eventSubscriptions.Remove(subscription);
            }
        }
    }
}