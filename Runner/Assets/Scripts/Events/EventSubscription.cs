using System;

namespace Assets.Scripts.Events
{
    public class EventSubscription
    {
        public Action<Event> Callback { get; }
        public string EventType { get; }

        public EventSubscription(Action<Event> callback, string eventType)
        {
            Callback = callback;
            EventType = eventType;
        }
    }
}