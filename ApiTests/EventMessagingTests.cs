using ApiTests.Tools;
using Backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ApiTests
{
    [TestFixture]
    public class EventMessagingTests
    {
        [Test]
        public async Task Test_GameSessionSendsEventsToClient()
        {
            using (var server = new TestServer(new WebHostBuilder().UseStartup<TestStartup>()))
            {
                var cts = new CancellationTokenSource();
                var client = new GameClient(server.CreateClient());

                var messageHandler = server.CreateHandler();
                var connection = new HubConnectionBuilder()
                    .WithUrl($"http://{server.Host}/gameSessionHub")
                    .WithMessageHandler(_ => messageHandler)
                    .WithTransport(Microsoft.AspNetCore.Http.Connections.TransportType.LongPolling)
                    .WithConsoleLogger()
                    .Build();
                await connection.StartAsync();

                var session = await client.StartSession(1);

                var channel = await connection.StreamAsChannelAsync<SessionEventModel>("StreamSessionEvents", session);
                var eventReader = ReadEvent(channel);
                Thread.Sleep(100);
                await client.JoinGame(session, "test player");

                var newEvent = await eventReader;
                Assert.NotNull(newEvent);
                Assert.AreEqual("Player test player joined the session.", newEvent.Description);
            }
        }

        private async Task<SessionEventModel> ReadEvent(ChannelReader<SessionEventModel> reader)
        {
            var cts = new CancellationTokenSource();
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            while (await reader.WaitToReadAsync(cts.Token))
            {
                while (reader.TryRead(out var receivedEvent))
                {
                    return receivedEvent;
                }
            }
            throw new Exception("No event");
        }
    }
}
