using DataStorage.Interfaces;
using System;
using System.Collections.Generic;
using DataStorage.DataObjects;
using System.Linq;
using Optional;
using Optional.Unsafe;
using DataStorage.DataObjects.Events;

namespace DataStorage.Implementations
{
    /// <summary>
    /// Mock implementation, this should be changed to actual thread safe implementation in future.
    /// 
    /// In actual implementation, somekind of service layer should probably handle event creation.
    /// </summary>
    public class MockGameSessionStorage : IGameSessionStorage, IGameSessionEventStorage, IGameSessionErrandStorage
    {
        private readonly Dictionary<Guid, List<Event>> _gameSessionEvents = new Dictionary<Guid, List<Event>>();
        private readonly List<GameSession> _sessions = new List<GameSession>();

        public Guid CreateSession(Game game, IEnumerable<Errand> errands)
        {
            var session = new GameSession
            {
                Id = Guid.NewGuid(),
                GameName = game.Name,
                Errands = errands.Select(CreateErrandCopy).ToList(),
                Players = new List<string>()
            };
            _sessions.Add(session);
            AddEvent(session.Id, new Event(session.Id, EventType.SessionCreated, "Session created", "Session created"));
            return session.Id;
        }

        private Errand CreateErrandCopy(Errand source)
        {
            return new Errand
            {
                Id = source.Id,
                Description = source.Description
            };
        }

        public IEnumerable<GameSession> GetAll()
        {
            return _sessions.ToList();
        }

        public void JoinSession(Guid sessionId, string playerName)
        {
            var session = _sessions
                .FirstOrDefault(s => s.Id == sessionId)
                .SomeNotNull()
                .ValueOrFailure($"No session found with ID {sessionId}");
            session.Players.Add(playerName);
            AddEvent(session.Id, new PlayerJoinedEvent(sessionId, playerName));
        }

        public Option<GameSession> GetSingle(Guid id)
        {
            return _sessions.FirstOrDefault(session => session.Id == id).SomeNotNull();
        }

        public List<Event> GetEvents(Guid sessionId)
        {
            try
            {
                return _gameSessionEvents[sessionId];
            }
            catch (Exception)
            {
                return new List<Event>();
            }
        }

        private void AddEvent(Guid sessionId, Event sessionEvent)
        {
            try
            {
                _gameSessionEvents[sessionId].Add(sessionEvent);
            }
            catch (Exception)
            {
                _gameSessionEvents[sessionId] = new List<Event>();
                _gameSessionEvents[sessionId].Add(sessionEvent);
            }
        }

        public Option<Errand> PopErrand(Guid sessionId)
        {
            var session = _sessions
                .FirstOrDefault(s => s.Id == sessionId)
                .SomeNotNull()
                .ValueOrFailure($"No session found with ID {sessionId}");

            //NOTE: Actual implemenation should be some stack-like implementation
            var errand = session.Errands.FirstOrDefault();
            if (errand != null)
            {
                session.Errands.RemoveAt(0);
                AddEvent(sessionId, new ErrandPoppedEvent(sessionId, errand.Description));
            }
            
            return errand.Some();
        }
    }
}
