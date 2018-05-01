using System;

namespace DataStorage.DataObjects.Events
{
    public class PlayerLeftEvent : Event
    {
        public string PlayerName { get; private set; }

        public PlayerLeftEvent(Guid sessionId, string playerName) : base(sessionId, EventType.PlayerLeft, $"Player {playerName} left", $"Player {playerName} left the session.")
        {
            PlayerName = playerName;
        }
    }
}
