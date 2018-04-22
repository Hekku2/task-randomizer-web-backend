using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using DataStorage.Interfaces;
using DataStorage.DataObjects;
using System.Linq;
using Optional.Unsafe;
using Optional.Linq;

namespace Backend.Controllers
{
    /// <summary>
    /// Game API
    /// </summary>
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class GameController : Controller
    {
        private readonly IGameStorage _gameStorage;

        public GameController(IGameStorage gameStorage)
        {
            _gameStorage = gameStorage;
        }

        /// <summary>
        /// Returns all games
        /// </summary>
        /// <returns>All games</returns>
        [HttpGet]
        public IEnumerable<GameModel> GetAll()
        {
            return _gameStorage.GetAll().Select(CreateGameModel).ToList();
        }

        /// <summary>
        /// Returns single game
        /// </summary>
        /// <param name="id">Id of game</param>
        /// <returns>Game</returns>
        [HttpGet("{id}")]
        public GameModel GetSingle(long id)
        {
            return _gameStorage
                .GetSingle(id)
                .Select(CreateGameModel)
                .ValueOrFailure($"No game exists with ID {id}");
        }

        private GameModel CreateGameModel(Game game)
        {
            return new GameModel
            {
                Id = game.Id,
                Name = game.Name
            };
        }
    }
}