using System;

namespace DataStorage.DataObjects.Events
{
    public class Event
    {
        public Guid SessionId { get; private set; }
        public EventType EventType { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        public Event(Guid sessionId, EventType type, string name, string description)
        {
            SessionId = sessionId;
            EventType = type;
            Name = name;
            Description = description;
        }   
    }
}
