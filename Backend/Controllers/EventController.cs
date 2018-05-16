using Backend.Models;
using DataStorage.DataObjects.Events;
using DataStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Backend.Controllers
{
    /// <summary>
    /// Event API
    /// This api handles Event related operations, mainly fetching events for sessions.
    /// </summary>
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class EventController : Controller
    {
        private readonly IGameSessionEventStorage _gameSessionEventStorage;

        public EventController(IGameSessionEventStorage gameSessionEventStorage)
        {
            _gameSessionEventStorage = gameSessionEventStorage;
        }

        /// <summary>
        /// Returns all events for game in order
        /// </summary>
        /// <param name="sessionId">ID of session</param>
        /// <returns>events in order</returns>
        [HttpGet("{sessionId}")]
        public List<SessionEventModel> GetEvents(Guid sessionId)
        {
            return _gameSessionEventStorage
                .GetEvents(sessionId)
                .Select(CreateSessionEvent)
                .ToList();
        }

        private SessionEventModel CreateSessionEvent(Event sessionEvent)
        {
            return new SessionEventModel
            {
                Timestamp = sessionEvent.Timestamp,
                SessionId = sessionEvent.SessionId,
                EventType = sessionEvent.EventType.ToString(),
                Name = sessionEvent.Name,
                Description = sessionEvent.Description,
                Context = sessionEvent.Context
            };
        }
    }
}
