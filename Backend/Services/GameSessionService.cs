using DataStorage.DataObjects;
using DataStorage.DataObjects.Events;
using DataStorage.Interfaces;
using Optional;
using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Backend.Services
{
    /// <summary>
    /// This service makes sure that correct items are added to storage and events are delivered to clients
    /// </summary>
    public class GameSessionService : IGameSessionService
    {
        private readonly IGameSessionStorage _gameSessionStorage;
        private readonly IGameStorage _gameStorage;
        private readonly IGameErrandStorage _gameErrandStorage;
        private readonly IGameSessionErrandStorage _gameSessionErrandStorage;
        private readonly IGameSessionEventStorage _gameSessionEventStorage;

        private ISubject<Event> _gameSessionEvents;
        
        public GameSessionService(IGameSessionStorage gameSessionStorage, IGameStorage gameStorage, IGameErrandStorage gameErrandStorage, IGameSessionErrandStorage gameSessionErrandStorage, IGameSessionEventStorage gameSessionEventStorage)
        {
            _gameSessionStorage = gameSessionStorage;
            _gameStorage = gameStorage;
            _gameErrandStorage = gameErrandStorage;
            _gameSessionErrandStorage = gameSessionErrandStorage;
            _gameSessionEventStorage = gameSessionEventStorage;

            _gameSessionEvents = new Subject<Event>();
        }

        public Guid StartSession(long gameId)
        {
            var game = _gameStorage
                .GetSingle(gameId)
                .ValueOrFailure($"No game exists with ID {gameId}");

            var errands = _gameErrandStorage.GetForGame(gameId);
            var sessionId = _gameSessionStorage.CreateSession(game, errands);
            var newEvent = new SessionCreatedEvent(sessionId, errands.Count());
            _gameSessionEvents.OnNext(newEvent);
            _gameSessionEventStorage.AddEvent(sessionId, newEvent);
            return sessionId;
        }

        public void JoinSession(Guid sessionId, string playerName)
        {
            _gameSessionStorage.JoinSession(sessionId, playerName);
            var newEvent = new PlayerJoinedEvent(sessionId, playerName);
            _gameSessionEvents.OnNext(newEvent);
            _gameSessionEventStorage.AddEvent(sessionId, newEvent);
        }

        public void LeaveSession(Guid sessionId, string playerName)
        {
            _gameSessionStorage.LeaveSession(sessionId, playerName);
            var newEvent = new PlayerLeftEvent(sessionId, playerName);
            _gameSessionEvents.OnNext(newEvent);
            _gameSessionEventStorage.AddEvent(sessionId, newEvent);
        }

        public Option<Errand> PopErrand(Guid sessionId, string playerName)
        {
            var errand = _gameSessionErrandStorage
                .PopErrand(sessionId);
            errand.MatchSome(value => 
            {
                var remainingEvents = _gameSessionErrandStorage.ErrandsRemaining(sessionId).ValueOrFailure("No session found");
                var newEvent = new ErrandPoppedEvent(sessionId, value.Description, remainingEvents, playerName);
                _gameSessionEvents.OnNext(newEvent);
                _gameSessionEventStorage.AddEvent(sessionId, newEvent);
            });
            
            return errand;
        }

        public IObservable<Event> StreamEvents(Guid sessionId)
        {
            return _gameSessionEvents.Where(ev => ev.SessionId == sessionId).AsObservable<Event>();
        }
    }
}
