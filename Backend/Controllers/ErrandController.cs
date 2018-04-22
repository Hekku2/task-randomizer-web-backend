using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Backend.Models;
using DataStorage.Interfaces;
using DataStorage.DataObjects;
using Optional.Unsafe;
using Optional.Linq;

namespace Backend.Controllers
{
    /// <summary>
    /// Errand API
    /// </summary>
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    public class ErrandController : Controller
    {
        private readonly IErrandStorage _taskStorage;
        private readonly IGameErrandStorage _gameErrandStorage;

        public ErrandController(IErrandStorage taskStorage, IGameErrandStorage gameErrandStorage)
        {
            _taskStorage = taskStorage;
            _gameErrandStorage = gameErrandStorage;
        }
        
        /// <summary>
        /// All errands
        /// </summary>
        /// <returns>All errands</returns>
        [HttpGet]
        public IEnumerable<ErrandModel> GetAll()
        {
            return _taskStorage.GetAll().Select(CreateTaskModel).ToList();
        }

        /// <summary>
        /// Errands for game
        /// </summary>
        /// <returns></returns>
        [HttpGet("game/{gameId}")]
        public IEnumerable<ErrandModel> GetForGame(long gameId)
        {
            return _gameErrandStorage.GetForGame(gameId).Select(CreateTaskModel).ToList();
        }

        /// <summary>
        /// Single errand
        /// </summary>
        /// <param name="id">ID of errand</param>
        /// <returns>Errand</returns>
        [HttpGet("{id}")]
        public ErrandModel GetSingle(long id)
        {
            return _taskStorage
                .GetSingle(id)
                .Select(CreateTaskModel)
                .ValueOrFailure($"No task exists with ID {id}");
        }

        private ErrandModel CreateTaskModel(Errand task)
        {
            return new ErrandModel
            {
                Id = task.Id,
                Description = task.Description
            };
        }
    }
}
