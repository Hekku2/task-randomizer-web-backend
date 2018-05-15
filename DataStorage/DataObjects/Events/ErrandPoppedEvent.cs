﻿using Newtonsoft.Json.Linq;
using System;

namespace DataStorage.DataObjects.Events
{
    public class ErrandPoppedEvent : Event
    {
        public ErrandPoppedEvent(Guid sessionId, string errandDescription, int errandsRemaining) : base(sessionId, EventType.ErrandPopped, "Errand popped", errandDescription)
        {
            Context = JObject.FromObject(new ErrandPoppedContext
            {
                ErrandsRemaining = errandsRemaining
            });
        }

        private class ErrandPoppedContext
        {
            public int ErrandsRemaining { get; set; }
        }
    }
}
