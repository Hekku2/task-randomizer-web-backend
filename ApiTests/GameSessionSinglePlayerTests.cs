using ApiTests.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTests
{
    [TestFixture]
    public class GameSessionSinglePlayerTests
    {
        private const long GameId = 1;

        [Test]
        public async Task Test_GameListIsUpdatedAfterCreation()
        {
            using (var server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>()))
            {
                var client = new GameClient(server.CreateClient());
                const string playerName = "Gamer 123";

                var sessionId = await client.StartSession(GameId);
                await client.JoinGame(sessionId, playerName);

                // Validate that session exists in ongoing sessions
                var session = await client.GetSession(sessionId);
                Assert.IsTrue(session.Players.Any(pl => pl == playerName));

                await client.PopErrand(sessionId);
                await client.PopErrand(sessionId);

                var events = await client.GetEvents(sessionId);
                Assert.AreEqual(4, events.Length);
                Assert.AreEqual("Session created", events[0].Name);
                Assert.AreEqual("Player Gamer 123 joined", events[1].Name);
                Assert.AreEqual("Errand popped", events[2].Name);
                Assert.AreEqual("Errand popped", events[3].Name);
            }
        }
    }
}
