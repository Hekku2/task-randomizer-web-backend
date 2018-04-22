using DataStorage.DataObjects;
using Optional;
using System;
using System.Collections.Generic;

namespace DataStorage.Interfaces
{
    public interface IGameSessionStorage
    {
        Guid CreateSession(Game game, IEnumerable<Errand> errands);
        void JoinSession(Guid sessionId, string playerName);
        IEnumerable<GameSession> GetAll();
        Option<GameSession> GetSingle(Guid id);
    }
}
