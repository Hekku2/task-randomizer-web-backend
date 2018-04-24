using ApiTests.Tools;
using Backend;
using Backend.Models;
using DataStorage.DataObjects.Events;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTests
{
    [TestFixture]
    public class GameSessionSinglePlayer
    {
        private const long GameId = 1;

        private TestServer _server;
        private HttpClient _client;

        private const string gameSessionUrl = "/api/v1/gameSession/";
        private const string eventsUrl = "/api/v1/event/";

        [SetUp]
        public void Setup()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder()
                .UseStartup<TestStartup>());
            _client = _server.CreateClient();
        }

        [Test]
        public async Task Test_GameListIsUpdatedAfterCreation()
        {
            const string playerName = "Gamer 123";

            var sessionId = await StartSession(GameId);
            await JoinGame(sessionId, playerName);

            // Validate that session exists in ongoing sessions
            var session = await GetSession(sessionId);
            Assert.IsTrue(session.Players.Any(pl => pl == playerName));

            await PopErrand(sessionId);
            await PopErrand(sessionId);

            var events = await GetEvents(sessionId);
            Assert.AreEqual(4, events.Length);
            Assert.AreEqual("Session created", events[0].Name);
            Assert.AreEqual("Player Gamer 123 joined", events[1].Name);
            Assert.AreEqual("Errand popped", events[2].Name);
            Assert.AreEqual("Errand popped", events[3].Name);
        }

        private async Task<Guid> StartSession(long gameId)
        {
            var model = new SessionSettingsModel
            {
                GameId = gameId
            };
            var response = await _client.PostAsJsonAsync(gameSessionUrl + "start", model);
            response.EnsureSuccessStatusCode();
            var sessionId = await response.ToObject<Guid>();
            Assert.NotNull(sessionId);
            Assert.AreNotEqual(Guid.Empty, sessionId);
            return sessionId;
        }

        private async Task<GameSessionModel> GetSession(Guid sessionId)
        {
            var response = await _client.GetAsync(gameSessionUrl + sessionId);
            response.EnsureSuccessStatusCode();
            var session = await response.ToObject<GameSessionModel>();
            Assert.NotNull(session);
            Assert.AreEqual(sessionId, session.Id);
            return session;
        }

        private async Task JoinGame(Guid sessionId, string player)
        {
            var model = new SessionJoinModel { SessionId = sessionId, PlayerName = player };
            var response = await _client.PostAsJsonAsync(gameSessionUrl + "join", model);
            response.EnsureSuccessStatusCode();
        }

        private async Task<ErrandModel> PopErrand(Guid sessionId)
        {
            var model = new SessionContextModel { SessionId = sessionId };
            var response = await _client.PostAsJsonAsync(gameSessionUrl + "popErrand", model);
            response.EnsureSuccessStatusCode();

            var errand = await response.ToObject<ErrandModel>();
            Assert.NotNull(errand);
            Assert.IsFalse(string.IsNullOrWhiteSpace(errand.Description));
            return errand;
        }

        private async Task<Event[]> GetEvents(Guid sessionId)
        {
            var response = await _client.GetAsync(eventsUrl + sessionId);
            response.EnsureSuccessStatusCode();
            var events = await response.ToObject<Event[]>();
            Assert.NotNull(events);
            foreach (var ev in events)
            {
                Assert.AreEqual(sessionId, ev.SessionId);
            }

            return events;
        }
    }
}
