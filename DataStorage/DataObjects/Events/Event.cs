using System;

namespace DataStorage.DataObjects.Events
{
    public class Event
    {
        public Guid SessionId { get; set; }
        public EventType EventType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public Event() { }

        public Event(Guid sessionId, EventType type, string name, string description)
        {
            SessionId = sessionId;
            EventType = type;
            Name = name;
            Description = description;
        }   
    }
}
