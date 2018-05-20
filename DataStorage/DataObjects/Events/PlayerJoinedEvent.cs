using Newtonsoft.Json.Linq;
using System;

namespace DataStorage.DataObjects.Events
{
    public class PlayerJoinedEvent : Event
    {
        public PlayerJoinedEvent(Guid sessionId, string playerName) : base(sessionId, EventType.PlayerJoined, $"Player {playerName} joined", $"Player {playerName} joined the session.")
        {
            Context = JObject.FromObject(new PlayerJoinedContext
            {
                PlayerName = playerName
            });
        }

        private class PlayerJoinedContext
        {
            public string PlayerName { get; set; }
        }
    }
}
