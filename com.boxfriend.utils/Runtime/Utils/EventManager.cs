using System;
using System.Collections.Generic;

namespace Boxfriend.Utils
{
    public class EventManager : Singleton<EventManager>
    {
        public delegate void Event(object arg, object sender);

        private readonly Dictionary<string, Event> _events = new ();

        public void RegisterEvent(string name)
        {
            if (_events.ContainsKey(name))
                throw new ArgumentException($"Event {name} already registered");

            _events.Add(name, null);
        }
        public void RegisterEvent (string name, Event callback)
        {
            if (_events.ContainsKey(name))
                throw new ArgumentException($"Event {name} already registered");

            _events.Add(name, callback);
        }

        public void UnregisterEvent(string name)
        {
            if (!_events.ContainsKey(name))
                throw new ArgumentException($"Event {name} not registered");

            _events.Remove(name);
        }

        public void SubscribeEvent (string name, Event callback)
        {
            if (!_events.ContainsKey(name))
                throw new ArgumentException($"Event {name} not registered");

            _events[name] += callback ?? throw new ArgumentNullException($"Event {name} callback is null");
        }

        public void UnsubscribeEvent (string name, Event callback)
        {
            if (!_events.ContainsKey(name))
                throw new ArgumentException($"Event {name} not registered");

            _events[name] -= callback ?? throw new ArgumentNullException($"Event {name} callback is null");
        }

        public void InvokeEvent (string name, object arg, object sender)
        {
            if (!_events.ContainsKey(name))
                throw new ArgumentException($"Event {name} not registered");

            _events[name]?.Invoke(arg, sender);
        }
    }
}
