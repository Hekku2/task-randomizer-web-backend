using Newtonsoft.Json.Linq;
using System;

namespace DataStorage.DataObjects.Events
{
    public class SessionCreatedEvent : Event
    {
        public SessionCreatedEvent(Guid sessionId, int errandsRemaining) : base(sessionId, EventType.SessionCreated, "Session created", "Session created")
        {
            Context = JObject.FromObject(new SessionCreatedContext
            {
                ErrandsRemaining = errandsRemaining
            });
        }

        private class SessionCreatedContext
        {
            public int ErrandsRemaining { get; set; }
        }
    }
}
