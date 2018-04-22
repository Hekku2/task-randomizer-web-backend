using Backend.Models;
using DataStorage.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Optional.Unsafe;
using Optional.Linq;
using System.Collections.Generic;
using System.Linq;
using DataStorage.DataObjects;
using System;

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
        private readonly IGameStorage _gameStorage;
        private readonly IGameErrandStorage _gameErrandStorage;
        private readonly IGameSessionErrandStorage _gameSessionErrandStorage;

        public GameSessionController(IGameSessionStorage gameSessionStorage, IGameStorage gameStorage, IGameErrandStorage gameErrandStorage, IGameSessionErrandStorage gameSessionErrandStorage)
        {
            _gameSessionStorage = gameSessionStorage;
            _gameStorage = gameStorage;
            _gameErrandStorage = gameErrandStorage;
            _gameSessionErrandStorage = gameSessionErrandStorage;
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
        public string StartSession(SessionSettingsModel settings)
        {
            var game = _gameStorage
                .GetSingle(settings.GameId)
                .ValueOrFailure($"No game exists with ID {settings.GameId}");

            var errands = _gameErrandStorage.GetForGame(settings.GameId);

            return _gameSessionStorage.CreateSession(game, errands).ToString();   
        }

        [HttpPost("join")]
        public void JoinSession(SessionJoinModel joinParameters)
        {
            _gameSessionStorage.JoinSession(joinParameters.SessionId, joinParameters.PlayerName);
        }

        /// <summary>
        /// Pops and returns errand for session
        /// </summary>
        /// <param name="parameters">session parameters</param>
        /// <returns>Errand</returns>
        [HttpPost("popErrand")]
        public ErrandModel PopErrand(SessionContextModel parameters)
        {
            var errand = _gameSessionErrandStorage
                .PopErrand(parameters.SessionId)
                .ValueOrFailure($"No errands for session with ID {parameters.SessionId}");

            return new ErrandModel
            {
                Id = errand.Id,
                Description = errand.Description
            };
        }
    }
}
