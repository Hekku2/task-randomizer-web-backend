using DataStorage.DataObjects;
using DataStorage.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Optional;

namespace DataStorage.Implementations
{
    /// <summary>
    /// Local mock implementation for Errand storage and GameErrandStorage
    /// </summary>
    public class MockStorage : IErrandStorage, IGameErrandStorage, IGameStorage
    {
        private readonly List<Game> _mockGames = new List<Game>()
        {
            new Game{Id = 1, Name = "Game 1"},
            new Game{Id = 2, Name = "Game 2"},
            new Game{Id = 3, Name = "Game 3"},
            new Game{Id = 4, Name = "Game 4"},
            new Game{Id = 5, Name = "Game 5"}
        };

        private readonly List<Errand> _mockErrands = new List<Errand>()
        {
            new Errand{Id = 1, Description = "Task1"},
            new Errand{Id = 2, Description = "Task2"},
            new Errand{Id = 3, Description = "Task3"},
            new Errand{Id = 4, Description = "Task4"},
            new Errand{Id = 5, Description = "Task5"}
        };

        private readonly Dictionary<long, List<Errand>> _mockGameErrands;

        public MockStorage()
        {
            _mockGameErrands = new Dictionary<long, List<Errand>>
            {
                { _mockGames[0].Id, new List<Errand>{ _mockErrands[0], _mockErrands[1] } },
                { _mockGames[1].Id, new List<Errand>{ _mockErrands[1], _mockErrands[0] } },
                { _mockGames[2].Id, new List<Errand>{ _mockErrands[3] } },
                { _mockGames[3].Id, new List<Errand>() },
                { _mockGames[4].Id, new List<Errand>{ _mockErrands[2], _mockErrands[3], _mockErrands[4], _mockErrands[0] } }
            };
        }

        IEnumerable<Errand> IErrandStorage.GetAll()
        {
            return _mockErrands.ToList();
        }

        IEnumerable<Errand> IGameErrandStorage.GetForGame(long gameId)
        {
            return _mockGameErrands[gameId].ToList();
        }

        Option<Errand> IErrandStorage.GetSingle(long id)
        {
            return _mockErrands.FirstOrDefault(task => task.Id == id).SomeNotNull();
        }

        IEnumerable<Game> IGameStorage.GetAll()
        {
            return _mockGames.ToList();
        }

        Option<Game> IGameStorage.GetSingle(long id)
        {
            return _mockGames.FirstOrDefault(task => task.Id == id).SomeNotNull();
        }
    }
}
