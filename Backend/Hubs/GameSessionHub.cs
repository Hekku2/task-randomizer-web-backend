using Microsoft.AspNetCore.SignalR;
using System;
using Backend.Services;
using DataStorage.DataObjects.Events;
using System.Reactive.Linq;
using Backend.Models;

namespace Backend.Hubs
{
    public class GameSessionHub : Hub
    {
        private readonly IGameSessionService _gameSessionService;

        public GameSessionHub(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        public IObservable<SessionEventModel> StreamSessionEvents(Guid sessionId)
        {
            return _gameSessionService.StreamEvents(sessionId).Select(CreateSessionEvent);
        }

        private SessionEventModel CreateSessionEvent(Event sessionEvent)
        {
            return new SessionEventModel
            {
                SessionId = sessionEvent.SessionId,
                EventType = sessionEvent.EventType.ToString(),
                Name = sessionEvent.Name,
                Description = sessionEvent.Description
            };
        }
    }
}
