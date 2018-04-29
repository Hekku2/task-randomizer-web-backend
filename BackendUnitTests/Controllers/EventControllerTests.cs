using Backend.Controllers;
using DataStorage.DataObjects.Events;
using DataStorage.Interfaces;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BackendUnitTests.Controllers
{
    [TestFixture]
    public class EventControllerTests : BaseControllerTests<EventController>
    {
        private IGameSessionEventStorage _mockStorage;

        protected override void OnSetup()
        {
            _mockStorage = Substitute.For<IGameSessionEventStorage>();
            Controller = new EventController(_mockStorage);
        }

        #region GetEvents

        [Test]
        public void Test_GetEvents_ReturnsNothingIfThereAreNoItems()
        {
            _mockStorage.GetEvents(Arg.Any<Guid>()).Returns(new List<Event>());

            var result = Controller.GetEvents(Guid.NewGuid());
            Assert.NotNull(result);
            Assert.IsFalse(result.Any());
        }

        [Test]
        public void Test_GetEvents_CorrectItems()
        {
            Guid session = Guid.NewGuid();
            var items = Enumerable.Range(1, 20).Select(i => new Event()
            {
                Name = "name",
                EventType = EventType.PlayerJoined,
                SessionId = session,
                Description = "game " + i
            }).ToList();
            _mockStorage.GetEvents(session).Returns(items);

            var result = Controller.GetEvents(session);
            Assert.NotNull(result);

            Assert.AreEqual(items.Count, result.Count());

            foreach (var item in items)
            {
                Assert.IsTrue(
                    result.Any(actual =>
                        actual.Name == item.Name &&
                        actual.SessionId == item.SessionId &&
                        actual.EventType == item.EventType.ToString() &&
                        actual.Description == item.Description));
            }
        }

        #endregion
    }
}
