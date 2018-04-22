using Backend.Controllers;
using DataStorage.Interfaces;
using DataStorage.DataObjects;
using NUnit.Framework;
using NSubstitute;
using System.Linq;
using Optional;
using Optional.Unsafe;

namespace BackendUnitTests.Controllers
{
    [TestFixture]
    public class GameControllerTests : BaseControllerTests<GameController>
    {
        private IGameStorage _mockGameStorage;

        protected override void OnSetup()
        {
            _mockGameStorage = Substitute.For<IGameStorage>();
            Controller = new GameController(_mockGameStorage);
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
            var items = Enumerable.Range(1, 20).Select(i => new Game()
            {
                Id = i,
                Name = "game " + i
            }).ToList();
            _mockGameStorage.GetAll().Returns(items);

            var result = Controller.GetAll();
            Assert.NotNull(result);

            Assert.AreEqual(items.Count, result.Count());

            foreach (var item in items)
            {
                Assert.IsTrue(
                    result.Any(actual => 
                        actual.Id == item.Id &&
                        actual.Name == item.Name));
            }
        }

        #endregion

        #region GetSingle

        [Test]
        public void Test_GetSingle_ReturnsCorrectItem()
        {
            var item = new Game
            {
                Id = 123,
                Name = "Name of the game"
            };
            _mockGameStorage.GetSingle(item.Id).Returns(item.Some());

            var result = Controller.GetSingle(item.Id);
            Assert.NotNull(result);

            Assert.AreEqual(item.Id, result.Id);
            Assert.AreEqual(item.Name, result.Name);
        }

        [Test]
        public void Test_GetSingle_ThrowsOptionValueMissingExceptionWhenItemsIsNotFound()
        {
            Assert.Throws<OptionValueMissingException>(() => Controller.GetSingle(123));
        }

        #endregion
    }
}
