using System;

namespace DataStorage.DataObjects.Events
{
    public class ErrandPoppedEvent : Event
    {
        public ErrandPoppedEvent(Guid sessionId, string errandDescription) : base(sessionId, EventType.ErrandPopped, "Errand popped", errandDescription)
        {
        }
    }
}
