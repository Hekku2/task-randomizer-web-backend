using Backend.Models;
using DataStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Optional.Unsafe;
using Optional.Linq;
using System.Collections.Generic;
using System.Linq;
using DataStorage.DataObjects;
using System;
using Backend.Services;

namespace Backend.Controllers
{
    /// <summary>
    /// Game Session API (starting, stopping, joining, listing)
    /// </summary>
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class GameSessionController : Controller
    {
        private readonly IGameSessionStorage _gameSessionStorage;
        private readonly IGameSessionService _gameSessionService;

        public GameSessionController(IGameSessionStorage gameSessionStorage, IGameSessionService gameSessionService)
        {
            _gameSessionStorage = gameSessionStorage;
            _gameSessionService = gameSessionService;
        }

        /// <summary>
        /// Returns all games
        /// </summary>
        /// <returns>All games</returns>
        [HttpGet]
        public IEnumerable<GameSessionModel> GetAll()
        {
            return _gameSessionStorage.GetAll().Select(CreateGameSessionModel).ToList();
        }

        [HttpGet("{id}")]
        public GameSessionModel GetSingle(Guid id)
        {
            return _gameSessionStorage
                .GetSingle(id)
                .Select(CreateGameSessionModel)
                .ValueOrFailure($"No session exists with ID {id}");
        }

        private GameSessionModel CreateGameSessionModel(GameSession session)
        {
            return new GameSessionModel
            {
                Id = session.Id,
                GameName = session.GameName,
                Errands = session.Errands.Select(CreateErrandModel).ToArray(),
                Players = session.Players.Select(p => p).ToArray()
            };
        }

        private ErrandModel CreateErrandModel(Errand errand)
        {
            return new ErrandModel
            {
                Id = errand.Id,
                Description = errand.Description
            };
        }

        /// <summary>
        /// Starts a game session with given settings.
        /// 
        /// Starting fails if given game doesn't exist.
        /// </summary>
        /// <param name="settings">Session settings</param>
        /// <returns>ID of created session. Can be used for navigating to session.</returns>
        [HttpPost("start")]
        public string StartSession([FromBody]SessionSettingsModel settings)
        {
            return _gameSessionService.StartSession(settings.GameId).ToString();  
        }

        [HttpPost("join")]
        public void JoinSession([FromBody]SessionContextModel joinParameters)
        {
            _gameSessionService.JoinSession(joinParameters.SessionId, joinParameters.PlayerName);
        }

        [HttpPost("leave")]
        public void LeaveSession([FromBody]SessionContextModel joinParameters)
        {
            _gameSessionService.LeaveSession(joinParameters.SessionId, joinParameters.PlayerName);
        }

        /// <summary>
        /// Pops and returns errand for session. If no errand exists, empty array is returned
        /// </summary>
        /// <param name="parameters">session parameters</param>
        /// <returns>Errand</returns>
        [HttpPost("popErrand")]
        public ErrandModel[] PopErrand([FromBody]SessionContextModel parameters)
        {
            return _gameSessionService
                .PopErrand(parameters.SessionId)
                .Select(CreateErrandModel)
                .ToEnumerable()
                .ToArray();
        }
    }
}
