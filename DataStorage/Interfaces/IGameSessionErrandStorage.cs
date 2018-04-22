using DataStorage.DataObjects;
using Optional;
using System;

namespace DataStorage.Interfaces
{
    /// <summary>
    /// Errand storage operations in GameSession context
    /// </summary>
    public interface IGameSessionErrandStorage
    {
        Option<Errand> PopErrand(Guid sessionId);
    }
}
