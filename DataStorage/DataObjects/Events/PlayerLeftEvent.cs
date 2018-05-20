using Newtonsoft.Json.Linq;
using System;

namespace DataStorage.DataObjects.Events
{
    public class PlayerLeftEvent : Event
    {
        public PlayerLeftEvent(Guid sessionId, string playerName) : base(sessionId, EventType.PlayerLeft, $"Player {playerName} left", $"Player {playerName} left the session.")
        {
            Context = JObject.FromObject(new PlayerLeftContext
            {
                PlayerName = playerName
            });
        }

        private class PlayerLeftContext
        {
            public string PlayerName { get; set; }
        }
    }
}
