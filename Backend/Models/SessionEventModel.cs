using System;

namespace Backend.Models
{
    public class SessionEventModel
    {
        public Guid SessionId { get; set; }
        public string EventType { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
