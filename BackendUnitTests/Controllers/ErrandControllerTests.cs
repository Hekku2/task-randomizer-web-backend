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
    public class ErrandControllerTests : BaseControllerTests<ErrandController>
    {
        private IGameErrandStorage _mockGameErrandStorage;
        private IErrandStorage _mockErrandStorage;

        protected override void OnSetup()
        {
            _mockErrandStorage = Substitute.For<IErrandStorage>();
            _mockGameErrandStorage = Substitute.For<IGameErrandStorage>();
            Controller = new ErrandController(_mockErrandStorage, _mockGameErrandStorage);
        }

        #region GetAll

        [Test]
        public void Test_GetAll_ReturnsNothingIfThereAreNoItems()
        {
            var result = Controller.GetAll();
            Assert.NotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Test_GetAll_ReturnsAllItems()
        {
            var items = Enumerable.Range(1, 20).Select(i => new Errand()
            {
                Id = i,
                Description = "game " + i
            }).ToList();
            _mockErrandStorage.GetAll().Returns(items);

            var result = Controller.GetAll();
            Assert.NotNull(result);

            Assert.AreEqual(items.Count, result.Count());

            foreach (var item in items)
            {
                Assert.IsTrue(
                    result.Any(actual =>
                        actual.Id == item.Id &&
                        actual.Description == item.Description));
            }
        }

        #endregion

        #region GetForGame

        [Test]
        public void Test_GetForGame_ReturnsNothingIfThereAreNoItems()
        {
            var result = Controller.GetForGame(123);
            Assert.NotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Test_GetForGame_ReturnsCorrectItems()
        {
            var gameId = 666;
            var items = Enumerable.Range(1, 20).Select(i => new Errand()
            {
                Id = i,
                Description = "game " + i
            }).ToList();
            _mockGameErrandStorage.GetForGame(gameId).Returns(items);

            var result = Controller.GetForGame(gameId);
            Assert.NotNull(result);

            Assert.AreEqual(items.Count, result.Count());

            foreach (var item in items)
            {
                Assert.IsTrue(
                    result.Any(actual =>
                        actual.Id == item.Id &&
                        actual.Description == item.Description));
            }
        }

        #endregion

        #region GetSingle

        [Test]
        public void Test_GetSingle_ReturnsCorrectItem()
        {
            var item = new Errand
            {
                Id = 123,
                Description = "Name of the game"
            };
            _mockErrandStorage.GetSingle(item.Id).Returns(item.Some());

            var result = Controller.GetSingle(item.Id);
            Assert.NotNull(result);

            Assert.AreEqual(item.Id, result.Id);
            Assert.AreEqual(item.Description, result.Description);
        }

        [Test]
        public void Test_GetSingle_ThrowsOptionValueMissingExceptionWhenItemsIsNotFound()
        {
            Assert.Throws<OptionValueMissingException>(() => Controller.GetSingle(123));
        }

        #endregion
    }
}
