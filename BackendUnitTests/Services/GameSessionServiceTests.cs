using Backend.Services;
using DataStorage.DataObjects;
using DataStorage.Interfaces;
using NSubstitute;
using NUnit.Framework;
using Optional;
using Optional.Unsafe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BackendUnitTests.Services
{
    [TestFixture]
    public class GameSessionServiceTests
    {
        private GameSessionService _service;

        private IGameSessionStorage _mockGameSessionStorage;
        private IGameStorage _mockGameStorage;
        private IGameErrandStorage _mockGameErrandStorage;
        private IGameSessionErrandStorage _mockGameSessionErrandStorage;
        private IGameSessionEventStorage _mockGameSessionEventStorage;

        [SetUp]
        public void Setup()
        {
            _mockGameSessionStorage = Substitute.For<IGameSessionStorage>();
            _mockGameStorage = Substitute.For<IGameStorage>();
            _mockGameErrandStorage = Substitute.For<IGameErrandStorage>();
            _mockGameSessionErrandStorage = Substitute.For<IGameSessionErrandStorage>();
            _mockGameSessionEventStorage = Substitute.For<IGameSessionEventStorage>();

            _service = new GameSessionService(_mockGameSessionStorage, _mockGameStorage, _mockGameErrandStorage, _mockGameSessionErrandStorage, _mockGameSessionEventStorage);
        }

        #region StartSession

        [Test]
        public void Test_StartSession_ReturnsIdForSession()
        {
            var gameId = 6667;

            var game = new Game() { Id = gameId };
            _mockGameStorage.GetSingle(game.Id).Returns(game.Some());

            var errands = Enumerable.Range(1, 6).Select(i => new Errand { Id = i, Description = "Errand " + i });
            _mockGameErrandStorage.GetForGame(game.Id).Returns(errands);

            var guid = Guid.NewGuid();
            _mockGameSessionStorage.CreateSession(game, errands).Returns(guid);

            var result = _service.StartSession(gameId);
            Assert.AreEqual(guid, result);
        }

        [Test]
        public void Test_StartSession_ThrowsOptionValueMissingExceptionWhenGamesIsNotFound()
        {
            var gameId = 666;
            var ex = Assert.Throws<OptionValueMissingException>(() => _service.StartSession(gameId));
            Assert.AreEqual("No game exists with ID 666", ex.Message);
        }

        #endregion

        #region JoinSession

        [Test]
        public void Test_JoinSession_JoinsSession()
        {
            var guid = Guid.NewGuid();
            var playerName = "playerplayer";
            _service.JoinSession(guid, playerName);

            _mockGameSessionStorage.Received().JoinSession(guid, playerName);
        }

        #endregion
    }
}
