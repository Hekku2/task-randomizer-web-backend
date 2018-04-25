using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Backend.Services;
using DataStorage.DataObjects.Events;

namespace Backend.Hubs
{
    public class GameSessionHub : Hub
    {
        private readonly IGameSessionService _gameSessionService;

        public GameSessionHub(IGameSessionService gameSessionService)
        {
            _gameSessionService = gameSessionService;
        }

        public IObservable<Event> StreamSessionEvents(Guid sessionId)
        {
            return _gameSessionService.StreamEvents(sessionId);
        }

        public async Task SendEvent(Guid sessionId, Event gameSessionEvent)
        {
            await Clients.All.SendAsync("ReceiveSessionEvent", sessionId, gameSessionEvent);
        }
    }
}
