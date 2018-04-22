using Backend.Controllers;
using DataStorage.Interfaces;
using DataStorage.DataObjects;
using NUnit.Framework;
using NSubstitute;
using System.Linq;
using Optional;
using Optional.Unsafe;
using Backend.Models;
using System;
using System.Collections.Generic;

namespace BackendUnitTests.Controllers
{
    [TestFixture]
    public class GameSessionControllerTests : BaseControllerTests<GameSessionController>
    {
        private IGameSessionStorage _mockGameSessionStorage;
        private IGameStorage _mockGameStorage;
        private IGameErrandStorage _mockGameErrandStorage;
        private IGameSessionErrandStorage _mockGameSessionErrandStorage;

        protected override void OnSetup()
        {
            _mockGameSessionStorage = Substitute.For<IGameSessionStorage>();
            _mockGameStorage = Substitute.For<IGameStorage>();
            _mockGameErrandStorage = Substitute.For<IGameErrandStorage>();
            _mockGameSessionErrandStorage = Substitute.For<IGameSessionErrandStorage>();
            Controller = new GameSessionController(_mockGameSessionStorage, _mockGameStorage, _mockGameErrandStorage, _mockGameSessionErrandStorage);
        }

        #region GetAll

        [Test]
        public void Test_GetAll_ReturnsNothingIfThereAreNoGames()
        {
            var result = Controller.GetAll();
            Assert.NotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Test_GetAll_ReturnsAllGames()
        {
            var items = Enumerable.Range(1, 20).Select(CreateSession).ToList();
            _mockGameSessionStorage.GetAll().Returns(items);

            var result = Controller.GetAll();
            Assert.NotNull(result);

            Assert.AreEqual(items.Count, result.Count());

            foreach (var item in items)
            {
                var match = result.FirstOrDefault(r => r.Id == item.Id);
                Assert.NotNull(match);
                Assert.AreEqual(item.GameName, match.GameName);
                Assert.NotNull(match.Errands);
                Assert.AreEqual(item.Errands.Count, match.Errands.Count());
                Assert.NotNull(match.Players);
                Assert.AreEqual(item.Players.Count, match.Players.Count());
                foreach (var player in item.Players)
                {
                    Assert.IsTrue(match.Players.Any(pl => pl == player));
                }
            }
        }

        private GameSession CreateSession(int index)
        {
            return new GameSession
            {
                Id = Guid.NewGuid(),
                GameName = "name of the game " + index,
                Players = Enumerable.Range(1, index).Select(j => $"Player {index} {j}").ToList(),
                Errands = Enumerable.Range(1, index).Select(CreateErrand).ToList()
            };
        }

        private Errand CreateErrand(int id)
        {
            return new Errand
            {
                Id = id,
                Description = "errand " + id
            };
        }

        #endregion

        #region GetSingle

        [Test]
        public void Test_GetSingle_ReturnsCorrectItem()
        {
            var item = new GameSession
            {
                Id = Guid.NewGuid(),
                GameName = "Name of game",
                Errands = new List<Errand>(),
                Players = new List<string>()
            };
            _mockGameSessionStorage.GetSingle(item.Id).Returns(item.Some());

            var result = Controller.GetSingle(item.Id);
            Assert.NotNull(result);

            Assert.AreEqual(item.Id, result.Id);
            Assert.AreEqual(item.GameName, result.GameName);
        }

        [Test]
        public void Test_GetSingle_ThrowsOptionValueMissingExceptionWhenItemsIsNotFound()
        {
            Assert.Throws<OptionValueMissingException>(() => Controller.GetSingle(Guid.NewGuid()));
        }

        #endregion

        #region StartSession

        [Test]
        public void Test_StartSession_ReturnsIdForSession()
        {
            var settings = new SessionSettingsModel
            {
                GameId = 8887
            };

            var game = new Game() { Id = settings.GameId };
            _mockGameStorage.GetSingle(game.Id).Returns(game.Some());

            var errands = Enumerable.Range(1, 6).Select(i => new Errand { Id = i, Description = "Errand " + i});
            _mockGameErrandStorage.GetForGame(game.Id).Returns(errands);

            var guid = Guid.NewGuid();
            _mockGameSessionStorage.CreateSession(game, errands).Returns(guid);

            var result = Controller.StartSession(settings);
            Assert.AreEqual(guid.ToString(), result);
        }

        [Test]
        public void Test_StartSession_ThrowsOptionValueMissingExceptionWhenGamesIsNotFound()
        {
            var settings = new SessionSettingsModel
            {
                GameId = 666
            };
            var ex = Assert.Throws<OptionValueMissingException>(() => Controller.StartSession(settings));
            Assert.AreEqual("No game exists with ID 666", ex.Message);
        }

        #endregion

        #region JoinSession

        [Test]
        public void Test_JoinSession_JoinsSession()
        {
            var join = new SessionJoinModel
            {
                SessionId = Guid.NewGuid(),
                PlayerName = "player player"
            };
            Controller.JoinSession(join);

            _mockGameSessionStorage.Received().JoinSession(join.SessionId, join.PlayerName);
        }

        #endregion
    }
}
