using Newtonsoft.Json.Linq;
using System;

namespace DataStorage.DataObjects.Events
{
    public class Event
    {
        public Guid SessionId { get; set; }
        public EventType EventType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }

        public JObject Context { get; set; }

        public Event()
        {
            Timestamp = DateTime.UtcNow;
        }

        public Event(Guid sessionId, EventType type, string name, string description)
        {
            Timestamp = DateTime.UtcNow;
            SessionId = sessionId;
            EventType = type;
            Name = name;
            Description = description;
        }   
    }
}
