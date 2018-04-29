using Backend.Models;
using NUnit.Framework;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiTests.Tools
{
    public class GameClient
    {
        private HttpClient _client;

        private const string gameSessionUrl = "/api/v1/gameSession/";
        private const string eventsUrl = "/api/v1/event/";

        public GameClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<Guid> StartSession(long gameId)
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

        public async Task<GameSessionModel> GetSession(Guid sessionId)
        {
            var response = await _client.GetAsync(gameSessionUrl + sessionId);
            response.EnsureSuccessStatusCode();
            var session = await response.ToObject<GameSessionModel>();
            Assert.NotNull(session);
            Assert.AreEqual(sessionId, session.Id);
            return session;
        }

        public async Task JoinGame(Guid sessionId, string player)
        {
            var model = new SessionJoinModel { SessionId = sessionId, PlayerName = player };
            var response = await _client.PostAsJsonAsync(gameSessionUrl + "join", model);
            response.EnsureSuccessStatusCode();
        }

        public async Task<ErrandModel> PopErrand(Guid sessionId)
        {
            var model = new SessionContextModel { SessionId = sessionId };
            var response = await _client.PostAsJsonAsync(gameSessionUrl + "popErrand", model);
            response.EnsureSuccessStatusCode();

            var errand = (await response.ToObject<ErrandModel[]>())[0];
            Assert.NotNull(errand);
            Assert.IsFalse(string.IsNullOrWhiteSpace(errand.Description));
            return errand;
        }

        public async Task<SessionEventModel[]> GetEvents(Guid sessionId)
        {
            var response = await _client.GetAsync(eventsUrl + sessionId);
            response.EnsureSuccessStatusCode();
            var events = await response.ToObject<SessionEventModel[]>();
            Assert.NotNull(events);
            foreach (var ev in events)
            {
                Assert.AreEqual(sessionId, ev.SessionId);
            }

            return events;
        }
    }
}
