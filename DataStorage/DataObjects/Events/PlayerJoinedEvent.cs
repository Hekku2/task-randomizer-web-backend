using System;

namespace DataStorage.DataObjects.Events
{
    public class PlayerJoinedEvent : Event
    {
        public string PlayerName { get; private set; }

        public PlayerJoinedEvent(Guid sessionId, string playerName) : base(sessionId, EventType.PlayerJoined, $"Player {playerName} joined", $"Player {playerName} joined the session.")
        {
            PlayerName = playerName;
        }
    }
}
