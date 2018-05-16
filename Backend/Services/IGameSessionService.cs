using DataStorage.DataObjects;
using DataStorage.DataObjects.Events;
using Optional;
using System;

namespace Backend.Services
{
    public interface IGameSessionService
    {
        Guid StartSession(long gameId);
        void JoinSession(Guid sessionId, string playerName);
        void LeaveSession(Guid sessionId, string playerName);
        Option<Errand> PopErrand(Guid sessionId, string playerName);
        IObservable<Event> StreamEvents(Guid sessionId);
    }
}
