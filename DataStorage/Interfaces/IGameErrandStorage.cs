using DataStorage.DataObjects;
using System.Collections.Generic;

namespace DataStorage.Interfaces
{
    /// <summary>
    /// Errands viewing, updating and editing related to Game
    /// </summary>
    public interface IGameErrandStorage
    {
        IEnumerable<Errand> GetForGame(long gameId);
    }
}
